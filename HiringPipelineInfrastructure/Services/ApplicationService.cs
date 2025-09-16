using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Interfaces.Repositories;
using HiringPipelineCore.Exceptions;
using HiringPipelineCore.DTOs;

namespace HiringPipelineInfrastructure.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IStageHistoryService _stageHistoryService;
        private readonly IAuditService _auditService;

        public ApplicationService(IApplicationRepository applicationRepository, IStageHistoryService stageHistoryService, IAuditService auditService)
        {
            _applicationRepository = applicationRepository;
            _stageHistoryService = stageHistoryService;
            _auditService = auditService;
        }

        public async Task<IEnumerable<Application>> GetAllAsync()
        {
            return await _applicationRepository.GetAllAsync();
        }

        public async Task<Application> GetByIdAsync(int id)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                throw new NotFoundException("Application", id);
            
            return application;
        }

        public async Task<Application> CreateAsync(Application application)
        {
            // Validate that candidate and requisition exist
            if (!await _applicationRepository.CandidateExistsAsync(application.CandidateId))
                throw new NotFoundException("Candidate", application.CandidateId);

            if (!await _applicationRepository.RequisitionExistsAsync(application.RequisitionId))
                throw new NotFoundException("Requisition", application.RequisitionId);

            var createdApplication = await _applicationRepository.AddAsync(application);
            return createdApplication;
        }

        public async Task<Application> UpdateAsync(int id, Application application)
        {
            var existingApplication = await _applicationRepository.GetByIdAsync(id);
            if (existingApplication == null)
                throw new NotFoundException("Application", id);

            // Store the old stage to check if it changed
            var oldStage = existingApplication.CurrentStage;

            // Update properties
            existingApplication.CandidateId = application.CandidateId;
            existingApplication.RequisitionId = application.RequisitionId;
            existingApplication.CurrentStage = application.CurrentStage;
            existingApplication.Status = application.Status;
            existingApplication.UpdatedAt = DateTime.UtcNow;

            var updatedApplication = await _applicationRepository.UpdateAsync(existingApplication);

            // If the stage changed, create a stage history entry
            if (oldStage != application.CurrentStage)
            {
                var stageHistory = new StageHistory
                {
                    ApplicationId = id,
                    FromStage = oldStage,
                    ToStage = application.CurrentStage,
                    MovedBy = "System", // Could be enhanced to track the actual user
                    MovedAt = DateTime.UtcNow
                };

                await _stageHistoryService.AddStageAsync(stageHistory);
            }

            return updatedApplication;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingApplication = await _applicationRepository.GetByIdAsync(id);
            if (existingApplication == null)
                throw new NotFoundException("Application", id);

            return await _applicationRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Application>> GetByCandidateIdAsync(int candidateId)
        {
            if (!await _applicationRepository.CandidateExistsAsync(candidateId))
                throw new NotFoundException("Candidate", candidateId);

            return await _applicationRepository.GetByCandidateIdAsync(candidateId);
        }

        public async Task<IEnumerable<Application>> GetByRequisitionIdAsync(int requisitionId)
        {
            if (!await _applicationRepository.RequisitionExistsAsync(requisitionId))
                throw new NotFoundException("Requisition", requisitionId);

            return await _applicationRepository.GetByRequisitionIdAsync(requisitionId);
        }

        public async Task<bool> CandidateExistsAsync(int candidateId)
        {
            return await _applicationRepository.CandidateExistsAsync(candidateId);
        }

        public async Task<bool> RequisitionExistsAsync(int requisitionId)
        {
            return await _applicationRepository.RequisitionExistsAsync(requisitionId);
        }

        public async Task ChangeStatusAsync(int id, ChangeApplicationStatusDto statusDto)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                throw new NotFoundException("Application", id);

            application.Status = statusDto.Status;
            application.UpdatedAt = DateTime.UtcNow;
            await _applicationRepository.UpdateAsync(application);
        }

        public async Task MoveToStageAsync(int id, MoveToStageDto stageDto)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                throw new NotFoundException("Application", id);

            application.CurrentStage = stageDto.ToStage;
            application.UpdatedAt = DateTime.UtcNow;
            await _applicationRepository.UpdateAsync(application);
        }

        public async Task UpdateCurrentStageAsync(int id, string newStage)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                throw new NotFoundException("Application", id);

            application.CurrentStage = newStage;
            application.UpdatedAt = DateTime.UtcNow;
            await _applicationRepository.UpdateAsync(application);
        }
    }
}
