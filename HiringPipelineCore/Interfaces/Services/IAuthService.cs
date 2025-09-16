using HiringPipelineCore.DTOs;

namespace HiringPipelineCore.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken, string? ipAddress = null);
        Task<bool> RevokeAllRefreshTokensForUserAsync(int userId, string? ipAddress = null);
    }
}
