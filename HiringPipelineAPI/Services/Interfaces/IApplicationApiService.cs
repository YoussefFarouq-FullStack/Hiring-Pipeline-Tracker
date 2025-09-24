using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Services.Interfaces
{
    public interface IApplicationApiService
    {
        Task<IEnumerable<ApplicationDto>> GetAllAsync();
        Task<ApplicationDetailDto> GetByIdAsync(int id);
        Task<ApplicationDto> CreateAsync(CreateApplicationDto createDto);
        Task<ApplicationDto> UpdateAsync(int id, UpdateApplicationDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ApplicationDto>> GetByCandidateIdAsync(int candidateId);
        Task<IEnumerable<ApplicationDto>> GetByRequisitionIdAsync(int requisitionId);
        Task<bool> CandidateExistsAsync(int candidateId);
        Task<bool> RequisitionExistsAsync(int requisitionId);
        Task ChangeStatusAsync(int id, ChangeApplicationStatusDto statusDto);
        Task MoveToStageAsync(int id, MoveToStageDto stageDto);
        
        // Search methods
        Task<SearchResponseDto<ApplicationDto>> SearchAsync(string? searchTerm, string? status, string? stage, string? department, int skip = 0, int take = 50);
    }
}
