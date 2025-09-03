using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Interfaces.Repositories;
using HiringPipelineCore.Exceptions;

namespace HiringPipelineInfrastructure.Services
{
    public class StageHistoryService : IStageHistoryService
    {
        private readonly IStageHistoryRepository _stageHistoryRepository;

        public StageHistoryService(IStageHistoryRepository stageHistoryRepository)
        {
            _stageHistoryRepository = stageHistoryRepository;
        }

        public async Task<IEnumerable<StageHistory>> GetByApplicationIdAsync(int applicationId)
        {
            return await _stageHistoryRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<StageHistory> AddStageAsync(StageHistory stageHistory)
        {
            var createdStageHistory = await _stageHistoryRepository.AddAsync(stageHistory);
            return createdStageHistory;
        }

        public async Task<IEnumerable<StageHistory>> GetAllAsync()
        {
            return await _stageHistoryRepository.GetAllAsync();
        }

        public async Task<StageHistory> GetByIdAsync(int id)
        {
            var stageHistory = await _stageHistoryRepository.GetByIdAsync(id);
            if (stageHistory == null)
                throw new NotFoundException("StageHistory", id);
            
            return stageHistory;
        }

        public async Task<StageHistory> UpdateAsync(int id, StageHistory stageHistory)
        {
            var existingStageHistory = await _stageHistoryRepository.GetByIdAsync(id);
            if (existingStageHistory == null)
                throw new NotFoundException("StageHistory", id);

            var updatedStageHistory = await _stageHistoryRepository.UpdateAsync(id, stageHistory);
            if (updatedStageHistory == null)
                throw new NotFoundException("StageHistory", id);
            
            return updatedStageHistory;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingStageHistory = await _stageHistoryRepository.GetByIdAsync(id);
            if (existingStageHistory == null)
                throw new NotFoundException("StageHistory", id);

            return await _stageHistoryRepository.DeleteAsync(id);
        }

        public async Task<bool> AnyAsync()
        {
            return await _stageHistoryRepository.AnyAsync();
        }

        public async Task DeleteAllAsync()
        {
            await _stageHistoryRepository.DeleteAllAsync();
        }

        public void ResetIdentitySeed()
        {
            _stageHistoryRepository.ResetIdentitySeed();
        }
    }
}
