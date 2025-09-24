using tablero_api.Models.DTOS;

namespace tablero_api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
        Task<RefreshResponseDto?> Refresh(RefreshRequestDto request);
        Task<string?> RegisterAsync(RegisterRequestDto request);

    }
}
