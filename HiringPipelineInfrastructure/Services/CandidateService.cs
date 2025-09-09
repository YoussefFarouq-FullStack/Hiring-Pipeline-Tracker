using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Interfaces.Repositories;
using HiringPipelineCore.Exceptions;

namespace HiringPipelineInfrastructure.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidateService(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        public async Task<IEnumerable<Candidate>> GetAllAsync()
        {
            return await _candidateRepository.GetAllAsync();
        }

        public async Task<Candidate> GetByIdAsync(int id)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);
            if (candidate == null)
                throw new NotFoundException("Candidate", id);
            
            return candidate;
        }

        public async Task<Candidate> CreateAsync(Candidate candidate)
        {
            var createdCandidate = await _candidateRepository.AddAsync(candidate);
            return createdCandidate;
        }

        public async Task<Candidate> UpdateAsync(int id, Candidate candidate)
        {
            var existingCandidate = await _candidateRepository.GetByIdAsync(id);
            if (existingCandidate == null)
                throw new NotFoundException("Candidate", id);

            // Update properties
            existingCandidate.FirstName = candidate.FirstName;
            existingCandidate.LastName = candidate.LastName;
            existingCandidate.Email = candidate.Email;
            existingCandidate.Phone = candidate.Phone;
            existingCandidate.ResumeFileName = candidate.ResumeFileName;
            existingCandidate.ResumeFilePath = candidate.ResumeFilePath;
            existingCandidate.Description = candidate.Description;
            existingCandidate.Skills = candidate.Skills;
            existingCandidate.Status = candidate.Status;
            existingCandidate.UpdatedAt = DateTime.UtcNow;

            var updatedCandidate = await _candidateRepository.UpdateAsync(existingCandidate);
            return updatedCandidate;
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
