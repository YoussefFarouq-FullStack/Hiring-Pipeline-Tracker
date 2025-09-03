using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Interfaces.Repositories;
using HiringPipelineCore.Exceptions;

namespace HiringPipelineInfrastructure.Services
{
    public class RequisitionService : IRequisitionService
    {
        private readonly IRequisitionRepository _requisitionRepository;

        public RequisitionService(IRequisitionRepository requisitionRepository)
        {
            _requisitionRepository = requisitionRepository;
        }

        public async Task<IEnumerable<Requisition>> GetAllAsync()
        {
            return await _requisitionRepository.GetAllAsync();
        }

        public async Task<Requisition> GetByIdAsync(int id)
        {
            var requisition = await _requisitionRepository.GetByIdAsync(id);
            if (requisition == null)
                throw new NotFoundException("Requisition", id);
            
            return requisition;
        }

        public async Task<Requisition> CreateAsync(Requisition requisition)
        {
            var createdRequisition = await _requisitionRepository.AddAsync(requisition);
            return createdRequisition;
        }

        public async Task<Requisition> UpdateAsync(int id, Requisition requisition)
        {
            var existingRequisition = await _requisitionRepository.GetByIdAsync(id);
            if (existingRequisition == null)
                throw new NotFoundException("Requisition", id);

            // Update properties
            existingRequisition.Title = requisition.Title;
            existingRequisition.Department = requisition.Department;
            existingRequisition.JobLevel = requisition.JobLevel;
            existingRequisition.Status = requisition.Status;
            existingRequisition.UpdatedAt = DateTime.UtcNow;

            var updatedRequisition = await _requisitionRepository.UpdateAsync(existingRequisition);
            return updatedRequisition;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingRequisition = await _requisitionRepository.GetByIdAsync(id);
            if (existingRequisition == null)
                throw new NotFoundException("Requisition", id);

            return await _requisitionRepository.DeleteAsync(id);
        }

        public async Task<bool> AnyAsync()
        {
            return await _requisitionRepository.AnyAsync();
        }

        public async Task DeleteAllAsync()
        {
            await _requisitionRepository.DeleteAllAsync();
        }

        public void ResetIdentitySeed()
        {
            _requisitionRepository.ResetIdentitySeed();
        }
    }
}
