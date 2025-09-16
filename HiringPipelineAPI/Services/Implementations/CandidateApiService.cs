using AutoMapper;
using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineAPI.Services.Interfaces;

namespace HiringPipelineAPI.Services.Implementations
{
    public class CandidateApiService : ICandidateApiService
    {
        private readonly ICandidateService _candidateService;
        private readonly IMapper _mapper;

        public CandidateApiService(ICandidateService candidateService, IMapper mapper)
        {
            _candidateService = candidateService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CandidateDto>> GetAllAsync()
        {
            var candidates = await _candidateService.GetAllAsync();
            return _mapper.Map<IEnumerable<CandidateDto>>(candidates);
        }

        public async Task<CandidateDetailDto> GetByIdAsync(int id)
        {
            var candidate = await _candidateService.GetByIdAsync(id);
            return _mapper.Map<CandidateDetailDto>(candidate);
        }

        public async Task<CandidateDto> CreateAsync(CreateCandidateDto createDto)
        {
            var candidate = _mapper.Map<Candidate>(createDto);
            var createdCandidate = await _candidateService.CreateAsync(candidate);
            return _mapper.Map<CandidateDto>(createdCandidate);
        }

        public async Task<CandidateDto> UpdateAsync(int id, UpdateCandidateDto updateDto)
        {
            var candidate = _mapper.Map<Candidate>(updateDto);
            var updatedCandidate = await _candidateService.UpdateAsync(id, candidate);
            return _mapper.Map<CandidateDto>(updatedCandidate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _candidateService.DeleteAsync(id);
        }

        public async Task<bool> AnyAsync()
        {
            return await _candidateService.AnyAsync();
        }

        public async Task DeleteAllAsync()
        {
            await _candidateService.DeleteAllAsync();
        }

        public void ResetIdentitySeed()
        {
            _candidateService.ResetIdentitySeed();
        }

        public async Task<FileUploadResultDto> UploadResumeAsync(int id, string fileName, byte[] fileContent)
        {
            return await _candidateService.UploadResumeAsync(id, fileName, fileContent);
        }

        public async Task AddSkillsAsync(int id, List<string> skills)
        {
            await _candidateService.AddSkillsAsync(id, skills);
        }

        public async Task ArchiveAsync(int id)
        {
            await _candidateService.ArchiveAsync(id);
        }
    }
}
