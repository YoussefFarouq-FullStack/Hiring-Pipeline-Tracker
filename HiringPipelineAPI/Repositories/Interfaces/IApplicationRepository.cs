using System.Collections.Generic;
using System.Threading.Tasks;
using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Repositories.Interfaces
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
    }
}
