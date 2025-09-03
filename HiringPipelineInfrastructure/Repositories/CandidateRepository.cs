using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiringPipelineInfrastructure.Data;
using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Repositories;

namespace HiringPipelineInfrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly HiringPipelineDbContext _context;

        public CandidateRepository(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Candidate>> GetAllAsync() =>
            await _context.Candidates.ToListAsync();

        public async Task<Candidate?> GetByIdAsync(int id) =>
            await _context.Candidates.FindAsync(id);

        public async Task<Candidate> AddAsync(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }

        public async Task<Candidate> UpdateAsync(Candidate candidate)
        {
            _context.Candidates.Update(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null) return false;
            
            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AnyAsync() =>
            await _context.Candidates.AnyAsync();

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
