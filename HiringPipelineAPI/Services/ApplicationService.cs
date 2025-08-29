using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Services.Implementations
{
    public class ApplicationService : IApplicationService
    {
        private readonly HiringPipelineDbContext _context;

        public ApplicationService(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Application>> GetAllAsync()
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Requisition)
                .ToListAsync();
        }

        public async Task<Application?> GetByIdAsync(int id)
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Requisition)
                .FirstOrDefaultAsync(a => a.ApplicationId == id);
        }

        public async Task<Application> CreateAsync(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<Application?> UpdateAsync(int id, Application application)
        {
            var existing = await _context.Applications.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(application);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Applications.FindAsync(id);
            if (existing == null) return false;

            _context.Applications.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Application>> GetByCandidateIdAsync(int candidateId)
        {
            return await _context.Applications
                .Where(a => a.CandidateId == candidateId)
                .Include(a => a.Requisition)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByRequisitionIdAsync(int requisitionId)
        {
            return await _context.Applications
                .Where(a => a.RequisitionId == requisitionId)
                .Include(a => a.Candidate)
                .ToListAsync();
        }

        public async Task<bool> CandidateExistsAsync(int candidateId)
        {
            return await _context.Candidates.AnyAsync(c => c.CandidateId == candidateId);
        }

        public async Task<bool> RequisitionExistsAsync(int requisitionId)
        {
            return await _context.Requisitions.AnyAsync(r => r.RequisitionId == requisitionId);
        }
    }
}
