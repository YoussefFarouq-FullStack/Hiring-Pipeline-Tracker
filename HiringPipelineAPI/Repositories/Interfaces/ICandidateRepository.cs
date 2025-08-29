using System.Collections.Generic;
using System.Threading.Tasks;
using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Repositories.Interfaces
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
    }
}
