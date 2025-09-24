using HiringPipelineCore.Entities;
using HiringPipelineCore.DTOs;

namespace HiringPipelineCore.Interfaces.Services
{
    public interface ICandidateService
    {
        Task<IEnumerable<Candidate>> GetAllAsync();
        Task<Candidate> GetByIdAsync(int id);
        Task<Candidate> CreateAsync(Candidate candidate);
        Task<Candidate> UpdateAsync(int id, Candidate candidate);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
        Task<FileUploadResultDto> UploadResumeAsync(int id, string fileName, byte[] fileContent);
        Task AddSkillsAsync(int id, List<string> skills);
        Task ArchiveAsync(int id);
        
        // Search methods
        Task<IEnumerable<Candidate>> SearchAsync(string? searchTerm, string? status, int? requisitionId, int skip = 0, int take = 50);
        Task<int> GetSearchCountAsync(string? searchTerm, string? status, int? requisitionId);
    }
}
