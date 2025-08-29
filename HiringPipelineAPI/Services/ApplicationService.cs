using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.Repositories.Interfaces;
using HiringPipelineAPI.DTOs;
using HiringPipelineAPI.Exceptions;
using AutoMapper;

namespace HiringPipelineAPI.Services.Implementations
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IMapper _mapper;

        public ApplicationService(IApplicationRepository applicationRepository, IMapper mapper)
        {
            _applicationRepository = applicationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApplicationDto>> GetAllAsync()
        {
            var applications = await _applicationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
        }

        public async Task<ApplicationDetailDto> GetByIdAsync(int id)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                throw new NotFoundException("Application", id);
            
            return _mapper.Map<ApplicationDetailDto>(application);
        }

        public async Task<ApplicationDto> CreateAsync(CreateApplicationDto createDto)
        {
            // Validate that candidate and requisition exist
            if (!await _applicationRepository.CandidateExistsAsync(createDto.CandidateId))
                throw new NotFoundException("Candidate", createDto.CandidateId);

            if (!await _applicationRepository.RequisitionExistsAsync(createDto.RequisitionId))
                throw new NotFoundException("Requisition", createDto.RequisitionId);

            var application = _mapper.Map<Application>(createDto);
            var createdApplication = await _applicationRepository.AddAsync(application);
            return _mapper.Map<ApplicationDto>(createdApplication);
        }

        public async Task<ApplicationDto> UpdateAsync(int id, UpdateApplicationDto updateDto)
        {
            var existingApplication = await _applicationRepository.GetByIdAsync(id);
            if (existingApplication == null)
                throw new NotFoundException("Application", id);

            _mapper.Map(updateDto, existingApplication);
            var updatedApplication = await _applicationRepository.UpdateAsync(existingApplication);
            return _mapper.Map<ApplicationDto>(updatedApplication);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingApplication = await _applicationRepository.GetByIdAsync(id);
            if (existingApplication == null)
                throw new NotFoundException("Application", id);

            return await _applicationRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ApplicationDto>> GetByCandidateIdAsync(int candidateId)
        {
            if (!await _applicationRepository.CandidateExistsAsync(candidateId))
                throw new NotFoundException("Candidate", candidateId);

            var applications = await _applicationRepository.GetByCandidateIdAsync(candidateId);
            return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
        }

        public async Task<IEnumerable<ApplicationDto>> GetByRequisitionIdAsync(int requisitionId)
        {
            if (!await _applicationRepository.RequisitionExistsAsync(requisitionId))
                throw new NotFoundException("Requisition", requisitionId);

            var applications = await _applicationRepository.GetByRequisitionIdAsync(requisitionId);
            return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
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
