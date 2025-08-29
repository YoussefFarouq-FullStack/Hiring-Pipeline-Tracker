using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.Repositories.Interfaces;
using HiringPipelineAPI.DTOs;
using HiringPipelineAPI.Exceptions;
using AutoMapper;

namespace HiringPipelineAPI.Services.Implementations
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IMapper _mapper;

        public CandidateService(ICandidateRepository candidateRepository, IMapper mapper)
        {
            _candidateRepository = candidateRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CandidateDto>> GetAllAsync()
        {
            var candidates = await _candidateRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CandidateDto>>(candidates);
        }

        public async Task<CandidateDetailDto> GetByIdAsync(int id)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);
            if (candidate == null)
                throw new NotFoundException("Candidate", id);
            
            return _mapper.Map<CandidateDetailDto>(candidate);
        }

        public async Task<CandidateDto> CreateAsync(CreateCandidateDto createDto)
        {
            var candidate = _mapper.Map<Candidate>(createDto);
            var createdCandidate = await _candidateRepository.AddAsync(candidate);
            return _mapper.Map<CandidateDto>(createdCandidate);
        }

        public async Task<CandidateDto> UpdateAsync(int id, UpdateCandidateDto updateDto)
        {
            var existingCandidate = await _candidateRepository.GetByIdAsync(id);
            if (existingCandidate == null)
                throw new NotFoundException("Candidate", id);

            _mapper.Map(updateDto, existingCandidate);
            var updatedCandidate = await _candidateRepository.UpdateAsync(existingCandidate);
            return _mapper.Map<CandidateDto>(updatedCandidate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingCandidate = await _candidateRepository.GetByIdAsync(id);
            if (existingCandidate == null)
                throw new NotFoundException("Candidate", id);

            return await _candidateRepository.DeleteAsync(id);
        }

        public async Task<bool> AnyAsync()
        {
            return await _candidateRepository.AnyAsync();
        }

        public async Task DeleteAllAsync()
        {
            await _candidateRepository.DeleteAllAsync();
        }

        public void ResetIdentitySeed()
        {
            _candidateRepository.ResetIdentitySeed();
        }
    }
}
