using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.Repositories.Interfaces;

namespace HiringPipelineAPI.Repositories.Implementations
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly HiringPipelineDbContext _context;

        public ApplicationRepository(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Application>> GetAllAsync() =>
            await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Requisition)
                .ToListAsync();

        public async Task<Application?> GetByIdAsync(int id) =>
            await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Requisition)
                .FirstOrDefaultAsync(a => a.ApplicationId == id);

        public async Task<Application> AddAsync(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<Application> UpdateAsync(Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null) return false;
            
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Application>> GetByCandidateIdAsync(int candidateId) =>
            await _context.Applications
                .Where(a => a.CandidateId == candidateId)
                .Include(a => a.Requisition)
                .ToListAsync();

        public async Task<IEnumerable<Application>> GetByRequisitionIdAsync(int requisitionId) =>
            await _context.Applications
                .Where(a => a.RequisitionId == requisitionId)
                .Include(a => a.Candidate)
                .ToListAsync();

        public async Task<bool> CandidateExistsAsync(int candidateId) =>
            await _context.Candidates.AnyAsync(c => c.CandidateId == candidateId);

        public async Task<bool> RequisitionExistsAsync(int requisitionId) =>
            await _context.Requisitions.AnyAsync(r => r.RequisitionId == requisitionId);
    }
}
