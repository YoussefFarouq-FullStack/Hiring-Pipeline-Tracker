using System.Collections.Generic;
using System.Threading.Tasks;
using HiringPipelineCore.Entities;

namespace HiringPipelineCore.Interfaces.Repositories
{
    public interface ICandidateRepository
    {
        Task<IEnumerable<Candidate>> GetAllAsync();
        Task<Candidate?> GetByIdAsync(int id);
        Task<Candidate> AddAsync(Candidate candidate);
        Task<Candidate> UpdateAsync(Candidate candidate);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
        
        // Search methods
        Task<IEnumerable<Candidate>> SearchAsync(string? searchTerm, string? status, int? requisitionId, int skip = 0, int take = 50);
        Task<int> GetSearchCountAsync(string? searchTerm, string? status, int? requisitionId);
    }
}
