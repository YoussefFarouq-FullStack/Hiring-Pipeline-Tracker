using HiringPipelineAPI.DTOs;
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
    }
}
