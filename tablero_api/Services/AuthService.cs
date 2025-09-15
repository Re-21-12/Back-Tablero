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
        private readonly IUsuarioRepository _usuarioRepository= usuarioRepository;
        private readonly IConfiguration _config = config;
        private readonly IService<Rol> _rolRepository = rolRepository;
        private readonly IService <RefreshToken> _refreshToken = refreshTokenRepository;
        private readonly CryptoHelper _cryptoHelper = cryptoHelper;

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request)
        {
            var usuario = await _usuarioRepository.GetUserWithRoleAsync(request.Nombre, request.Contrasena);
            if (usuario == null)
                return null;
            var token = GenerateJwtToken(usuario);
            string nombreRol = "";
            if (usuario.Rol != null)
            {
                //Esto es por que en el repository se usa el includes para buscar y eso facilita la inclusion
                nombreRol = usuario.Rol.Nombre;
            }
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
            };
            await _refreshToken.CreateAsync(refreshToken);
            RolDto rolDto = new(nombreRol);

            return new LoginResponseDto(token, usuario.Nombre, rolDto, refreshToken.Token);
            
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

            var ontrasenaCifrada = BCrypt.Net.BCrypt.HashPassword(request.Contrasena);

            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Contrasena = ontrasenaCifrada,
                Id_Rol = rol.Id_Rol
            };

            await _usuarioRepository.AddAsync(usuario);
            return "Usuario registrado correctamente.";
        }

    }
}
