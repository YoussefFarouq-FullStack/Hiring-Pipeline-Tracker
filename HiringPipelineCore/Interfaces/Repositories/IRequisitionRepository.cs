using System.Collections.Generic;
using System.Threading.Tasks;
using HiringPipelineCore.Entities;

namespace HiringPipelineCore.Interfaces.Repositories
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
        
        // Search methods
        Task<IEnumerable<Requisition>> SearchAsync(string? searchTerm, string? status, string? department, string? priority, string? employmentType, string? experienceLevel, bool? isDraft, int skip = 0, int take = 50);
        Task<int> GetSearchCountAsync(string? searchTerm, string? status, string? department, string? priority, string? employmentType, string? experienceLevel, bool? isDraft);
    }
}
