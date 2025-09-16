using HiringPipelineCore.Entities;
using HiringPipelineCore.DTOs;

namespace HiringPipelineCore.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<IEnumerable<Application>> GetAllAsync();
        Task<Application> GetByIdAsync(int id);
        Task<Application> CreateAsync(Application application);
        Task<Application> UpdateAsync(int id, Application application);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Application>> GetByCandidateIdAsync(int candidateId);
        Task<IEnumerable<Application>> GetByRequisitionIdAsync(int requisitionId);
        Task<bool> CandidateExistsAsync(int candidateId);
        Task<bool> RequisitionExistsAsync(int requisitionId);
        Task ChangeStatusAsync(int id, ChangeApplicationStatusDto statusDto);
        Task MoveToStageAsync(int id, MoveToStageDto stageDto);
        Task UpdateCurrentStageAsync(int id, string newStage);
    }
}
