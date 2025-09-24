using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Interfaces.Repositories;
using HiringPipelineCore.Exceptions;
using HiringPipelineCore.DTOs;

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

        public async Task<FileUploadResultDto> UploadResumeAsync(int id, string fileName, byte[] fileContent)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);
            if (candidate == null)
                throw new NotFoundException("Candidate", id);

            // In a real implementation, you would save the file to disk or cloud storage
            // For now, we'll just update the candidate record with file information
            var filePath = $"uploads/resumes/{id}_{fileName}";
            
            candidate.ResumeFileName = fileName;
            candidate.ResumeFilePath = filePath;
            candidate.UpdatedAt = DateTime.UtcNow;
            
            await _candidateRepository.UpdateAsync(candidate);

            return new FileUploadResultDto
            {
                FileName = fileName,
                FileSize = fileContent.Length,
                FilePath = filePath
            };
        }

        public async Task AddSkillsAsync(int id, List<string> skills)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);
            if (candidate == null)
                throw new NotFoundException("Candidate", id);

            // Add new skills to existing skills
            var existingSkills = string.IsNullOrEmpty(candidate.Skills) 
                ? new List<string>() 
                : candidate.Skills.Split(',').Select(s => s.Trim()).ToList();

            var newSkills = skills.Where(s => !existingSkills.Contains(s)).ToList();
            existingSkills.AddRange(newSkills);

            candidate.Skills = string.Join(", ", existingSkills);
            candidate.UpdatedAt = DateTime.UtcNow;
            
            await _candidateRepository.UpdateAsync(candidate);
        }

        public async Task ArchiveAsync(int id)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);
            if (candidate == null)
                throw new NotFoundException("Candidate", id);

            if (candidate.Status == "Archived")
                throw new InvalidOperationException("Candidate is already archived");

            candidate.Status = "Archived";
            candidate.UpdatedAt = DateTime.UtcNow;
            await _candidateRepository.UpdateAsync(candidate);
        }

        public async Task<IEnumerable<Candidate>> SearchAsync(string? searchTerm, string? status, int? requisitionId, int skip = 0, int take = 50)
        {
            return await _candidateRepository.SearchAsync(searchTerm, status, requisitionId, skip, take);
        }

        public async Task<int> GetSearchCountAsync(string? searchTerm, string? status, int? requisitionId)
        {
            return await _candidateRepository.GetSearchCountAsync(searchTerm, status, requisitionId);
        }
    }
}
