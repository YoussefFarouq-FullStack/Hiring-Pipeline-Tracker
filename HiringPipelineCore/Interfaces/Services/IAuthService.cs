using HiringPipelineCore.DTOs;

namespace HiringPipelineCore.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    }
}
