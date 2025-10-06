using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Repositories.Interfaces;
using tablero_api.Services.Interfaces;
using tablero_api.Utils;

namespace tablero_api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _config;
        private readonly IService<Rol> _rolRepository;
        private readonly CryptoHelper _cryptoHelper;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IService<Rol> rolRepository,
            IConfiguration config,
            CryptoHelper cryptoHelper,
            IHttpClientFactory httpClientFactory)
        {
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
            _config = config;
            _cryptoHelper = cryptoHelper;
            _httpClientFactory = httpClientFactory;
        }

        // Authenticate against Keycloak using Resource Owner Password Credentials (not recommended for public clients)
        // This preserves the existing /login endpoint that accepts username/password. Prefer using Authorization Code flow from the frontend.
        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request)
        {
            var keycloak = _config.GetSection("Keycloak");
            var tokenEndpoint = keycloak["Authority"]?.TrimEnd('/') + "/protocol/openid-connect/token";
            var clientId = keycloak["ClientId"];
            var clientSecret = keycloak["ClientSecret"];

            var client = _httpClientFactory.CreateClient();
            var pairs = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "password"),
                new("client_id", clientId ?? string.Empty),
                new("username", request.Nombre),
                new("password", request.Contrasena)
            };
            if (!string.IsNullOrEmpty(clientSecret))
                pairs.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

            var content = new FormUrlEncodedContent(pairs);
            var resp = await client.PostAsync(tokenEndpoint, content);
            if (!resp.IsSuccessStatusCode)
                return null;

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var accessToken = doc.RootElement.GetProperty("access_token").GetString();
            var refreshToken = doc.RootElement.GetProperty("refresh_token").GetString();
            var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();

            // Obtener información del usuario desde el token (no lo validamos aquí pues Jwt middleware lo hará en requests)
            // También sincronizamos usuario local y permisos
            var usuario = await SyncUserFromAccessTokenAsync(accessToken);

            string nombreRol = usuario?.Rol?.Nombre ?? string.Empty;
            List<PermisoDto> permisos = new();
            if (usuario?.Rol?.Permisos != null)
            {
                permisos = usuario.Rol.Permisos.Select(p => new PermisoDto(p.Nombre, p.Id_Rol)).ToList();
            }

            var rolDto = new RolDto(usuario?.Id_Rol ?? 0, nombreRol);

            return new LoginResponseDto(accessToken, expiresIn, usuario?.Nombre, rolDto, refreshToken ?? string.Empty, permisos);
        }

        public async Task<RefreshResponseDto?> Refresh(RefreshRequestDto request)
        {
            var keycloak = _config.GetSection("Keycloak");
            var tokenEndpoint = keycloak["Authority"]?.TrimEnd('/') + "/protocol/openid-connect/token";
            var clientId = keycloak["ClientId"];
            var clientSecret = keycloak["ClientSecret"];

            var client = _httpClientFactory.CreateClient();
            var pairs = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "refresh_token"),
                new("client_id", clientId ?? string.Empty),
                new("refresh_token", request.RefreshToken)
            };
            if (!string.IsNullOrEmpty(clientSecret))
                pairs.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

            var content = new FormUrlEncodedContent(pairs);
            var resp = await client.PostAsync(tokenEndpoint, content);
            if (!resp.IsSuccessStatusCode)
                return null;

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var accessToken = doc.RootElement.GetProperty("access_token").GetString();
            var newRefresh = doc.RootElement.GetProperty("refresh_token").GetString();

            return new RefreshResponseDto(accessToken ?? string.Empty, newRefresh ?? string.Empty);
        }

        public async Task<string?> RegisterAsync(RegisterRequestDto request)
        {
            // Nota: El flujo correcto para registrar usuarios es mediante Keycloak Admin API.
            // Aquí sólo creamos un usuario local (sin password válido en Keycloak).
            var usuarioExistente = await _usuarioRepository.GetByUsernameAsync(request.Nombre);
            if (usuarioExistente != null)
                return null;

            var rol = await _rolRepository.GetByPredicateAsync(r => r.Nombre == request.Rol.Nombre);
            if (rol == null)
                return null;

            var contrasenaCifrada = BCrypt.Net.BCrypt.HashPassword(request.Contrasena);

            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Contrasena = contrasenaCifrada,
                Id_Rol = rol.Id_Rol
            };

            await _usuarioRepository.AddAsync(usuario);
            return "Usuario registrado localmente. Para registrar en Keycloak use la Admin API";
        }

        private async Task<Usuario?> SyncUserFromAccessTokenAsync(string? accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return null;

            // Decodificar token para obtener preferred_username y roles
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            var nombre = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var roles = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            if (string.IsNullOrEmpty(nombre))
                return null;

            var usuario = await _usuarioRepository.GetByUsernameAsync(nombre);
            if (usuario == null)
            {
                // Crear usuario local sin password
                var rolNombre = roles.FirstOrDefault() ?? "Cliente";
                var rol = await _rolRepository.GetByPredicateAsync(r => r.Nombre == rolNombre) ?? await _rolRepository.GetByPredicateAsync(r => r.Nombre == "Cliente");

                usuario = new Usuario
                {
                    Nombre = nombre,
                    Contrasena = string.Empty,
                    Id_Rol = rol?.Id_Rol ?? 0
                };
                await _usuarioRepository.AddAsync(usuario);
            }
            else
            {
                // Actualizar rol si es necesario
                var rolNombre = roles.FirstOrDefault();
                if (!string.IsNullOrEmpty(rolNombre))
                {
                    var rol = await _rolRepository.GetByPredicateAsync(r => r.Nombre == rolNombre);
                    if (rol != null && usuario.Id_Rol != rol.Id_Rol)
                    {
                        usuario.Id_Rol = rol.Id_Rol;
                        await _usuarioRepository.UpdateAsync(usuario);
                    }
                }
            }

            return usuario;
        }
    }
}
