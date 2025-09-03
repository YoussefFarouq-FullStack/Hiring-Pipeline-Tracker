using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Interfaces.Repositories;
using HiringPipelineCore.Exceptions;

namespace HiringPipelineInfrastructure.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
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

            // Update properties
            existingApplication.CandidateId = application.CandidateId;
            existingApplication.RequisitionId = application.RequisitionId;
            existingApplication.CurrentStage = application.CurrentStage;
            existingApplication.Status = application.Status;
            existingApplication.UpdatedAt = DateTime.UtcNow;

            var updatedApplication = await _applicationRepository.UpdateAsync(existingApplication);
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
    }
}
