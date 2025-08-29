using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Services.Interfaces
{
    public interface IStageHistoryService
    {
        Task<IEnumerable<StageHistory>> GetByApplicationIdAsync(int applicationId);
        Task<StageHistory> AddStageAsync(StageHistory history);
        Task<IEnumerable<StageHistory>> GetAllAsync();
        Task<StageHistory?> GetByIdAsync(int id);
        Task<StageHistory?> UpdateAsync(int id, StageHistory history);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
    }
}
