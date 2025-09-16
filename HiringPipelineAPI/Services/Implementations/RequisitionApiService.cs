using AutoMapper;
using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineAPI.Services.Interfaces;

namespace HiringPipelineAPI.Services.Implementations
{
    public class RequisitionApiService : IRequisitionApiService
    {
        private readonly IRequisitionService _requisitionService;
        private readonly IMapper _mapper;

        public RequisitionApiService(IRequisitionService requisitionService, IMapper mapper)
        {
            _requisitionService = requisitionService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RequisitionDto>> GetAllAsync()
        {
            var requisitions = await _requisitionService.GetAllAsync();
            return _mapper.Map<IEnumerable<RequisitionDto>>(requisitions);
        }

        public async Task<RequisitionDetailDto> GetByIdAsync(int id)
        {
            var requisition = await _requisitionService.GetByIdAsync(id);
            return _mapper.Map<RequisitionDetailDto>(requisition);
        }

        public async Task<RequisitionDto> CreateAsync(CreateRequisitionDto createDto)
        {
            var requisition = _mapper.Map<Requisition>(createDto);
            var createdRequisition = await _requisitionService.CreateAsync(requisition);
            return _mapper.Map<RequisitionDto>(createdRequisition);
        }

        public async Task<RequisitionDto> UpdateAsync(int id, UpdateRequisitionDto updateDto)
        {
            var requisition = _mapper.Map<Requisition>(updateDto);
            var updatedRequisition = await _requisitionService.UpdateAsync(id, requisition);
            return _mapper.Map<RequisitionDto>(updatedRequisition);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _requisitionService.DeleteAsync(id);
        }

        public async Task<bool> AnyAsync()
        {
            return await _requisitionService.AnyAsync();
        }

        public async Task DeleteAllAsync()
        {
            await _requisitionService.DeleteAllAsync();
        }

        public void ResetIdentitySeed()
        {
            _requisitionService.ResetIdentitySeed();
        }

        public async Task PublishAsync(int id)
        {
            await _requisitionService.PublishAsync(id);
        }

        public async Task CloseAsync(int id)
        {
            await _requisitionService.CloseAsync(id);
        }
    }
}
