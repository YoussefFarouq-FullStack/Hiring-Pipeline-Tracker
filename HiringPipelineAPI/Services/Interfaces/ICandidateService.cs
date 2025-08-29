using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Services.Interfaces
{
    public interface ICandidateService
    {
        Task<IEnumerable<Candidate>> GetAllAsync();
        Task<Candidate?> GetByIdAsync(int id);
        Task<Candidate> CreateAsync(Candidate candidate);
        Task<Candidate?> UpdateAsync(int id, Candidate candidate);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
    }
}
