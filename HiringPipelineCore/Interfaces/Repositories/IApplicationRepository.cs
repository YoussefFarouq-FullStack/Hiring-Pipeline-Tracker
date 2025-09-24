using System.Collections.Generic;
using System.Threading.Tasks;
using HiringPipelineCore.Entities;

namespace HiringPipelineCore.Interfaces.Repositories
{
    public interface IApplicationRepository
    {
        Task<IEnumerable<Application>> GetAllAsync();
        Task<Application?> GetByIdAsync(int id);
        Task<Application> AddAsync(Application application);
        Task<Application> UpdateAsync(Application application);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Application>> GetByCandidateIdAsync(int candidateId);
        Task<IEnumerable<Application>> GetByRequisitionIdAsync(int requisitionId);
        Task<bool> CandidateExistsAsync(int candidateId);
        Task<bool> RequisitionExistsAsync(int requisitionId);
        
        // Search methods
        Task<IEnumerable<Application>> SearchAsync(string? searchTerm, string? status, string? stage, string? department, int skip = 0, int take = 50);
        Task<int> GetSearchCountAsync(string? searchTerm, string? status, string? stage, string? department);
    }
}
