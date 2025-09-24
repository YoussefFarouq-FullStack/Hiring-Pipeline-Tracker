using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Services.Interfaces
{
    public interface IRequisitionApiService
    {
        Task<IEnumerable<RequisitionDto>> GetAllAsync();
        Task<RequisitionDetailDto> GetByIdAsync(int id);
        Task<RequisitionDto> CreateAsync(CreateRequisitionDto createDto);
        Task<RequisitionDto> UpdateAsync(int id, UpdateRequisitionDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync();
        Task DeleteAllAsync();
        void ResetIdentitySeed();
        Task PublishAsync(int id);
        Task CloseAsync(int id);
        
        // Search methods
        Task<SearchResponseDto<RequisitionDto>> SearchAsync(string? searchTerm, string? status, string? department, string? priority, string? employmentType, string? experienceLevel, bool? isDraft, int skip = 0, int take = 50);
    }
}
