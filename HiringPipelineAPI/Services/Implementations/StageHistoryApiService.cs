using AutoMapper;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineAPI.Services.Interfaces;

namespace HiringPipelineAPI.Services.Implementations
{
    public class StageHistoryApiService : IStageHistoryApiService
    {
        private readonly IStageHistoryService _stageHistoryService;
        private readonly IMapper _mapper;

        public StageHistoryApiService(IStageHistoryService stageHistoryService, IMapper mapper)
        {
            _stageHistoryService = stageHistoryService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StageHistoryDto>> GetByApplicationIdAsync(int applicationId)
        {
            var stageHistories = await _stageHistoryService.GetByApplicationIdAsync(applicationId);
            return _mapper.Map<IEnumerable<StageHistoryDto>>(stageHistories);
        }

        public async Task<StageHistoryDto> AddStageAsync(CreateStageHistoryDto createDto)
        {
            var stageHistory = _mapper.Map<StageHistory>(createDto);
            var createdStageHistory = await _stageHistoryService.AddStageAsync(stageHistory);
            return _mapper.Map<StageHistoryDto>(createdStageHistory);
        }

        public async Task<IEnumerable<StageHistoryDto>> GetAllAsync()
        {
            var stageHistories = await _stageHistoryService.GetAllAsync();
            return _mapper.Map<IEnumerable<StageHistoryDto>>(stageHistories);
        }

        public async Task<StageHistoryDetailDto> GetByIdAsync(int id)
        {
            var stageHistory = await _stageHistoryService.GetByIdAsync(id);
            return _mapper.Map<StageHistoryDetailDto>(stageHistory);
        }

        public async Task<StageHistoryDto> UpdateAsync(int id, CreateStageHistoryDto updateDto)
        {
            var stageHistory = _mapper.Map<StageHistory>(updateDto);
            var updatedStageHistory = await _stageHistoryService.UpdateAsync(id, stageHistory);
            return _mapper.Map<StageHistoryDto>(updatedStageHistory);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _stageHistoryService.DeleteAsync(id);
        }

        public async Task<bool> AnyAsync()
        {
            return await _stageHistoryService.AnyAsync();
        }

        public async Task DeleteAllAsync()
        {
            await _stageHistoryService.DeleteAllAsync();
        }

        public void ResetIdentitySeed()
        {
            _stageHistoryService.ResetIdentitySeed();
        }
    }
}
