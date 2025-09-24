using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiringPipelineInfrastructure.Data;
using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Repositories;

namespace HiringPipelineInfrastructure.Repositories
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

        public async Task<IEnumerable<Application>> SearchAsync(string? searchTerm, string? status, string? stage, string? department, int skip = 0, int take = 50)
        {
            var query = _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Requisition)
                .AsQueryable();

            // Apply most selective filters first
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(stage))
            {
                query = query.Where(a => a.CurrentStage == stage);
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(a => a.Requisition != null && a.Requisition.Department == department);
            }

            // Apply search term filter last (least selective)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                if (term.Length > 0)
                {
                    query = query.Where(a => 
                        (a.Candidate != null && (a.Candidate.FirstName.ToLower().Contains(term) || 
                                               a.Candidate.LastName.ToLower().Contains(term) ||
                                               a.Candidate.Email.ToLower().Contains(term))) ||
                        (a.Requisition != null && (a.Requisition.Title.ToLower().Contains(term) ||
                                                 (a.Requisition.Description != null && a.Requisition.Description.ToLower().Contains(term)))) ||
                        (a.CurrentStage != null && a.CurrentStage.ToLower().Contains(term)));
                }
            }

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetSearchCountAsync(string? searchTerm, string? status, string? stage, string? department)
        {
            var query = _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Requisition)
                .AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(a => 
                    (a.Candidate != null && (a.Candidate.FirstName.ToLower().Contains(term) || 
                                           a.Candidate.LastName.ToLower().Contains(term) ||
                                           a.Candidate.Email.ToLower().Contains(term))) ||
                    (a.Requisition != null && (a.Requisition.Title.ToLower().Contains(term) ||
                                             (a.Requisition.Description != null && a.Requisition.Description.ToLower().Contains(term)))) ||
                    (a.CurrentStage != null && a.CurrentStage.ToLower().Contains(term)));
            }

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status == status);
            }

            // Apply stage filter
            if (!string.IsNullOrWhiteSpace(stage))
            {
                query = query.Where(a => a.CurrentStage == stage);
            }

            // Apply department filter
            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(a => a.Requisition != null && a.Requisition.Department == department);
            }

            return await query.CountAsync();
        }
    }
}
