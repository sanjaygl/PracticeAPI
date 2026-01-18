using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PracticeAPI.Configuration;
using PracticeAPI.Data;
using PracticeAPI.DTOs;
using PracticeAPI.Services.Interfaces;

namespace PracticeAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ApplicationDbContext context,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string ipAddress)
        {
            try
            {
                // Find user by username
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == request.Username);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found - {Username}", request.Username);
                    return null;
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login failed: User is inactive - {Username}", request.Username);
                    return null;
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid password - {Username}", request.Username);
                    return null;
                }

                // Generate tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = await _tokenService.CreateRefreshTokenAsync(user.Id, ipAddress);

                _logger.LogInformation("User logged in successfully - {Username}", request.Username);

                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresIn = _jwtSettings.AccessTokenExpiryMinutes * 60, // Convert to seconds
                    Username = user.Username,
                    Role = user.Role.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", request.Username);
                throw;
            }
        }

        public async Task<RefreshTokenResponseDto?> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            try
            {
                // Validate refresh token
                var validToken = await _tokenService.ValidateRefreshTokenAsync(refreshToken);

                if (validToken == null)
                {
                    _logger.LogWarning("Refresh token validation failed");
                    return null;
                }

                // Revoke old refresh token
                await _tokenService.RevokeRefreshTokenAsync(refreshToken);

                // Generate new tokens
                var newAccessToken = _tokenService.GenerateAccessToken(validToken.User);
                var newRefreshToken = await _tokenService.CreateRefreshTokenAsync(validToken.UserId, ipAddress);

                _logger.LogInformation("Token refreshed successfully for user {UserId}", validToken.UserId);

                return new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    ExpiresIn = _jwtSettings.AccessTokenExpiryMinutes * 60
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                throw;
            }
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (token == null)
                {
                    _logger.LogWarning("Logout failed: Refresh token not found");
                    return false;
                }

                await _tokenService.RevokeRefreshTokenAsync(refreshToken);
                _logger.LogInformation("User logged out successfully - UserId: {UserId}", token.UserId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                throw;
            }
        }
    }
}
