using AutoMapper;
using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineAPI.Services.Interfaces;

namespace HiringPipelineAPI.Services.Implementations
{
    public class ApplicationApiService : IApplicationApiService
    {
        private readonly IApplicationService _applicationService;
        private readonly IMapper _mapper;

        public ApplicationApiService(IApplicationService applicationService, IMapper mapper)
        {
            _applicationService = applicationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApplicationDto>> GetAllAsync()
        {
            var applications = await _applicationService.GetAllAsync();
            return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
        }

        public async Task<ApplicationDetailDto> GetByIdAsync(int id)
        {
            var application = await _applicationService.GetByIdAsync(id);
            return _mapper.Map<ApplicationDetailDto>(application);
        }

        public async Task<ApplicationDto> CreateAsync(CreateApplicationDto createDto)
        {
            var application = _mapper.Map<Application>(createDto);
            var createdApplication = await _applicationService.CreateAsync(application);
            return _mapper.Map<ApplicationDto>(createdApplication);
        }

        public async Task<ApplicationDto> UpdateAsync(int id, UpdateApplicationDto updateDto)
        {
            var application = _mapper.Map<Application>(updateDto);
            var updatedApplication = await _applicationService.UpdateAsync(id, application);
            return _mapper.Map<ApplicationDto>(updatedApplication);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _applicationService.DeleteAsync(id);
        }

        public async Task<IEnumerable<ApplicationDto>> GetByCandidateIdAsync(int candidateId)
        {
            var applications = await _applicationService.GetByCandidateIdAsync(candidateId);
            return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
        }

        public async Task<IEnumerable<ApplicationDto>> GetByRequisitionIdAsync(int requisitionId)
        {
            var applications = await _applicationService.GetByRequisitionIdAsync(requisitionId);
            return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
        }

        public async Task<bool> CandidateExistsAsync(int candidateId)
        {
            return await _applicationService.CandidateExistsAsync(candidateId);
        }

        public async Task<bool> RequisitionExistsAsync(int requisitionId)
        {
            return await _applicationService.RequisitionExistsAsync(requisitionId);
        }

        public async Task ChangeStatusAsync(int id, ChangeApplicationStatusDto statusDto)
        {
            await _applicationService.ChangeStatusAsync(id, statusDto);
        }

        public async Task MoveToStageAsync(int id, MoveToStageDto stageDto)
        {
            await _applicationService.MoveToStageAsync(id, stageDto);
        }
    }
}
