using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Services.Interfaces
{
    public interface ICandidateApiService
    {
        Task<IEnumerable<CandidateDto>> GetAllAsync();
        Task<CandidateDetailDto> GetByIdAsync(int id);
        Task<CandidateDto> CreateAsync(CreateCandidateDto createDto);
        Task<CandidateDto> UpdateAsync(int id, UpdateCandidateDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
    }
}
