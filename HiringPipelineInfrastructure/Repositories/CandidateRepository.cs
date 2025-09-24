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

        public async Task<IEnumerable<Candidate>> SearchAsync(string? searchTerm, string? status, int? requisitionId, int skip = 0, int take = 50)
        {
            var query = _context.Candidates.AsQueryable();

            // Apply status filter first (most selective)
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.Status == status);
            }

            // Apply requisition filter (through applications)
            if (requisitionId.HasValue)
            {
                query = query.Where(c => _context.Applications.Any(a => a.CandidateId == c.CandidateId && a.RequisitionId == requisitionId.Value));
            }

            // Apply search term filter last (least selective)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                if (term.Length > 0)
                {
                    query = query.Where(c => 
                        c.FirstName.ToLower().Contains(term) ||
                        c.LastName.ToLower().Contains(term) ||
                        c.Email.ToLower().Contains(term) ||
                        (c.Phone != null && c.Phone.Contains(term)) ||
                        (c.Skills != null && c.Skills.ToLower().Contains(term)) ||
                        (c.Description != null && c.Description.ToLower().Contains(term)));
                }
            }

            return await query
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetSearchCountAsync(string? searchTerm, string? status, int? requisitionId)
        {
            var query = _context.Candidates.AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(c => 
                    c.FirstName.ToLower().Contains(term) ||
                    c.LastName.ToLower().Contains(term) ||
                    c.Email.ToLower().Contains(term) ||
                    (c.Phone != null && c.Phone.Contains(term)) ||
                    (c.Skills != null && c.Skills.ToLower().Contains(term)) ||
                    (c.Description != null && c.Description.ToLower().Contains(term)));
            }

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.Status == status);
            }

            // Apply requisition filter (through applications)
            if (requisitionId.HasValue)
            {
                query = query.Where(c => _context.Applications.Any(a => a.CandidateId == c.CandidateId && a.RequisitionId == requisitionId.Value));
            }

            return await query.CountAsync();
        }
    }
}
