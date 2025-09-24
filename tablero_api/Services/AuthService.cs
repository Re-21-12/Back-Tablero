using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Repositories.Interfaces;
using tablero_api.Services.Interfaces;
using tablero_api.Utils;

namespace tablero_api.Services
{
    public class AuthService(
        IUsuarioRepository usuarioRepository,
        IService<Rol> rolRepository,
        IConfiguration config,
        CryptoHelper cryptoHelper,
        IService<RefreshToken> refreshTokenRepository
        ) : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IConfiguration _config = config;
        private readonly IService<Rol> _rolRepository = rolRepository;
        private readonly IService<RefreshToken> _refreshToken = refreshTokenRepository;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request)
        {
            var usuario = await _usuarioRepository.GetUserWithRoleAndPermissionsAsync(request.Nombre, request.Contrasena);
            if (usuario == null)
                return null;

            var token = GenerateJwtToken(usuario);
            string nombreRol = "";
            List<PermisoDto> permisos = new();

            if (usuario.Rol != null)
            {
                nombreRol = usuario.Rol.Nombre;

                // Obtener permisos del rol
                if (usuario.Rol.Permisos != null && usuario.Rol.Permisos.Any())
                {
                    permisos = usuario.Rol.Permisos.Select(p => new PermisoDto(p.Nombre, p.Id_Rol)).ToList();
                }
            }

            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UsuarioId = usuario.Id_Usuario,
                IsRevoked = false
            };
            await _refreshToken.CreateAsync(refreshToken);

            RolDto rolDto = new(usuario.Id_Rol, nombreRol);

            return new LoginResponseDto(
                token,
                Convert.ToInt32(_config["Jwt:ExpiresInMinutes"]),
                usuario.Nombre,
                rolDto,
                refreshToken.Token,
                permisos
            );
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "YourSuperSecretKey123!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            string nombreRol = "";
            if (usuario.Rol != null)
            {
                nombreRol = usuario.Rol.Nombre;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, nombreRol),
                new Claim("Id", _cryptoHelper.Encrypt(usuario.Id_Usuario.ToString()))
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<RefreshResponseDto> Refresh(RefreshRequestDto request)
        {
            RefreshToken existingToken = await _refreshToken.GetByPredicateAsync(rt => rt.Token == request.RefreshToken);
            if (existingToken == null || existingToken.ExpiryDate < DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid or expired refresh token");
            }

            var usuario = await _usuarioRepository.GetByIdAsync(existingToken.UsuarioId);
            if (usuario == null)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
            var newJwtToken = GenerateJwtToken(usuario);
            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UsuarioId = usuario.Id_Usuario,
                IsRevoked = false
            };
            await _refreshToken.CreateAsync(newRefreshToken);
            await _refreshToken.DeleteAsync(existingToken.Id);
            return new RefreshResponseDto(newJwtToken, newRefreshToken.Token);
        }

        public async Task<string?> RegisterAsync(RegisterRequestDto request)
        {
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
            return "Usuario registrado correctamente.";
        }
    }
}
