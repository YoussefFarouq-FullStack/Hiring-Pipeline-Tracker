using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Services.Implementations
{
    public class CandidateService : ICandidateService
    {
        private readonly HiringPipelineDbContext _context;

        public CandidateService(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Candidate>> GetAllAsync()
        {
            return await _context.Candidates.ToListAsync();
        }

        public async Task<Candidate?> GetByIdAsync(int id)
        {
            return await _context.Candidates.FindAsync(id);
        }

        public async Task<Candidate> CreateAsync(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }

        public async Task<Candidate?> UpdateAsync(int id, Candidate candidate)
        {
            var existing = await _context.Candidates.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(candidate);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Candidates.FindAsync(id);
            if (existing == null) return false;

            _context.Candidates.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AnyAsync()
        {
            return await _context.Candidates.AnyAsync();
        }

        public async Task DeleteAllAsync()
        {
            var candidates = await _context.Candidates.ToListAsync();
            if (candidates.Any())
            {
                _context.Candidates.RemoveRange(candidates);
                await _context.SaveChangesAsync();
            }
        }

        public void ResetIdentitySeed()
        {
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Candidates', RESEED, 0)");
        }
    }
}
