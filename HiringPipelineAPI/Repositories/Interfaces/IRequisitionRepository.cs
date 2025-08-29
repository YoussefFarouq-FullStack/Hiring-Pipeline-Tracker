using System.Collections.Generic;
using System.Threading.Tasks;
using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Repositories.Interfaces
{
    public interface IRequisitionRepository
    {
        Task<IEnumerable<Requisition>> GetAllAsync();
        Task<Requisition?> GetByIdAsync(int id);
        Task<Requisition> AddAsync(Requisition requisition);
        Task<Requisition> UpdateAsync(Requisition requisition);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
    }
}
