using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.Repositories.Interfaces;
using HiringPipelineAPI.DTOs;
using HiringPipelineAPI.Exceptions;
using AutoMapper;

namespace HiringPipelineAPI.Services.Implementations
{
    public class RequisitionService : IRequisitionService
    {
        private readonly IRequisitionRepository _requisitionRepository;
        private readonly IMapper _mapper;

        public RequisitionService(IRequisitionRepository requisitionRepository, IMapper mapper)
        {
            _requisitionRepository = requisitionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RequisitionDto>> GetAllAsync()
        {
            var requisitions = await _requisitionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RequisitionDto>>(requisitions);
        }

        public async Task<RequisitionDetailDto> GetByIdAsync(int id)
        {
            var requisition = await _requisitionRepository.GetByIdAsync(id);
            if (requisition == null)
                throw new NotFoundException("Requisition", id);
            
            return _mapper.Map<RequisitionDetailDto>(requisition);
        }

        public async Task<RequisitionDto> CreateAsync(CreateRequisitionDto createDto)
        {
            var requisition = _mapper.Map<Requisition>(createDto);
            var createdRequisition = await _requisitionRepository.AddAsync(requisition);
            return _mapper.Map<RequisitionDto>(createdRequisition);
        }

        public async Task<RequisitionDto> UpdateAsync(int id, UpdateRequisitionDto updateDto)
        {
            var existingRequisition = await _requisitionRepository.GetByIdAsync(id);
            if (existingRequisition == null)
                throw new NotFoundException("Requisition", id);

            _mapper.Map(updateDto, existingRequisition);
            var updatedRequisition = await _requisitionRepository.UpdateAsync(existingRequisition);
            return _mapper.Map<RequisitionDto>(updatedRequisition);
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
