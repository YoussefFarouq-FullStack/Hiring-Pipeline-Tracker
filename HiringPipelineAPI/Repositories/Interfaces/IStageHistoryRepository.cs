using System.Collections.Generic;
using System.Threading.Tasks;
using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Repositories.Interfaces
{
    public interface IStageHistoryRepository
    {
        Task<IEnumerable<StageHistory>> GetByApplicationIdAsync(int applicationId);
        Task<StageHistory> AddAsync(StageHistory history);
        Task<IEnumerable<StageHistory>> GetAllAsync();
        Task<StageHistory?> GetByIdAsync(int id);
        Task<StageHistory?> UpdateAsync(int id, StageHistory history);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
    }
}
