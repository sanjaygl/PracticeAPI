using PracticeAPI.DTOs;

namespace PracticeAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string ipAddress);
        Task<RefreshTokenResponseDto?> RefreshTokenAsync(string refreshToken, string ipAddress);
        Task<bool> LogoutAsync(string refreshToken);
    }
}
