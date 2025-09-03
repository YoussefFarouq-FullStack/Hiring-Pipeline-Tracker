using HiringPipelineCore.Entities;

namespace HiringPipelineCore.Interfaces.Services
{
    public interface IStageHistoryService
    {
        Task<IEnumerable<StageHistory>> GetByApplicationIdAsync(int applicationId);
        Task<StageHistory> AddStageAsync(StageHistory stageHistory);
        Task<IEnumerable<StageHistory>> GetAllAsync();
        Task<StageHistory> GetByIdAsync(int id);
        Task<StageHistory> UpdateAsync(int id, StageHistory stageHistory);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
    }
}
