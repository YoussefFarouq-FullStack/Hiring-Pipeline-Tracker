using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Services.Interfaces
{
    public interface IRequisitionService
    {
        Task<IEnumerable<Requisition>> GetAllAsync();
        Task<Requisition?> GetByIdAsync(int id);
        Task<Requisition> CreateAsync(Requisition requisition);
        Task<Requisition?> UpdateAsync(int id, Requisition requisition);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
    }
}
