using HiringPipelineCore.Entities;

namespace HiringPipelineCore.Interfaces.Services
{
    public interface IRequisitionService
    {
        Task<IEnumerable<Requisition>> GetAllAsync();
        Task<Requisition> GetByIdAsync(int id);
        Task<Requisition> CreateAsync(Requisition requisition);
        Task<Requisition> UpdateAsync(int id, Requisition requisition);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
        Task PublishAsync(int id);
        Task CloseAsync(int id);
    }
}
