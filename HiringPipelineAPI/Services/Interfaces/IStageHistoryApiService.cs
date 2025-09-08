using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Services.Interfaces
{
    public interface IStageHistoryApiService
    {
        Task<IEnumerable<StageHistoryDto>> GetByApplicationIdAsync(int applicationId);
        Task<StageHistoryDto> AddStageAsync(CreateStageHistoryDto createDto);
        Task<IEnumerable<StageHistoryDto>> GetAllAsync();
        Task<StageHistoryDetailDto> GetByIdAsync(int id);
        Task<StageHistoryDto> UpdateAsync(int id, CreateStageHistoryDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
    }
}
