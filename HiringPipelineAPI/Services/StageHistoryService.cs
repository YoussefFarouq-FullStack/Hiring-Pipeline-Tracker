using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.Repositories.Interfaces;
using HiringPipelineAPI.DTOs;
using HiringPipelineAPI.Exceptions;
using AutoMapper;

namespace HiringPipelineAPI.Services.Implementations
{
    public class StageHistoryService : IStageHistoryService
    {
        private readonly IStageHistoryRepository _stageHistoryRepository;
        private readonly IMapper _mapper;

        public StageHistoryService(IStageHistoryRepository stageHistoryRepository, IMapper mapper)
        {
            _stageHistoryRepository = stageHistoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StageHistoryDto>> GetByApplicationIdAsync(int applicationId)
        {
            var stageHistories = await _stageHistoryRepository.GetByApplicationIdAsync(applicationId);
            return _mapper.Map<IEnumerable<StageHistoryDto>>(stageHistories);
        }

        public async Task<StageHistoryDto> AddStageAsync(CreateStageHistoryDto createDto)
        {
            var stageHistory = _mapper.Map<StageHistory>(createDto);
            var createdStageHistory = await _stageHistoryRepository.AddAsync(stageHistory);
            return _mapper.Map<StageHistoryDto>(createdStageHistory);
        }

        public async Task<IEnumerable<StageHistoryDto>> GetAllAsync()
        {
            var stageHistories = await _stageHistoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StageHistoryDto>>(stageHistories);
        }

        public async Task<StageHistoryDetailDto> GetByIdAsync(int id)
        {
            var stageHistory = await _stageHistoryRepository.GetByIdAsync(id);
            if (stageHistory == null)
                throw new NotFoundException("StageHistory", id);
            
            return _mapper.Map<StageHistoryDetailDto>(stageHistory);
        }

        public async Task<StageHistoryDto> UpdateAsync(int id, CreateStageHistoryDto updateDto)
        {
            var existingStageHistory = await _stageHistoryRepository.GetByIdAsync(id);
            if (existingStageHistory == null)
                throw new NotFoundException("StageHistory", id);

            var stageHistory = _mapper.Map<StageHistory>(updateDto);
            var updatedStageHistory = await _stageHistoryRepository.UpdateAsync(id, stageHistory);
            if (updatedStageHistory == null)
                throw new NotFoundException("StageHistory", id);
            
            return _mapper.Map<StageHistoryDto>(updatedStageHistory);
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
