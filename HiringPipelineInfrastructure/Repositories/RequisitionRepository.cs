using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiringPipelineInfrastructure.Data;
using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Repositories;

namespace HiringPipelineInfrastructure.Repositories
{
    public class RequisitionRepository : IRequisitionRepository
    {
        private readonly HiringPipelineDbContext _context;

        public RequisitionRepository(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Requisition>> GetAllAsync() =>
            await _context.Requisitions.ToListAsync();

        public async Task<Requisition?> GetByIdAsync(int id) =>
            await _context.Requisitions.FindAsync(id);

        public async Task<Requisition> AddAsync(Requisition requisition)
        {
            _context.Requisitions.Add(requisition);
            await _context.SaveChangesAsync();
            return requisition;
        }

        public async Task<Requisition> UpdateAsync(Requisition requisition)
        {
            _context.Requisitions.Update(requisition);
            await _context.SaveChangesAsync();
            return requisition;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var requisition = await _context.Requisitions.FindAsync(id);
            if (requisition == null) return false;
            
            _context.Requisitions.Remove(requisition);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AnyAsync() =>
            await _context.Requisitions.AnyAsync();

        public async Task DeleteAllAsync()
        {
            var requisitions = await _context.Requisitions.ToListAsync();
            if (requisitions.Any())
            {
                _context.Requisitions.RemoveRange(requisitions);
                await _context.SaveChangesAsync();
            }
        }

        public void ResetIdentitySeed()
        {
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Requisitions', RESEED, 0)");
        }

        public async Task<IEnumerable<Requisition>> SearchAsync(string? searchTerm, string? status, string? department, string? priority, string? employmentType, string? experienceLevel, bool? isDraft, int skip = 0, int take = 50)
        {
            var query = _context.Requisitions.AsQueryable();

            // Apply most selective filters first
            if (isDraft.HasValue)
            {
                query = query.Where(r => r.IsDraft == isDraft.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                query = query.Where(r => r.Priority == priority);
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(r => r.Department == department);
            }

            if (!string.IsNullOrWhiteSpace(employmentType))
            {
                query = query.Where(r => r.EmploymentType == employmentType);
            }

            if (!string.IsNullOrWhiteSpace(experienceLevel))
            {
                query = query.Where(r => r.ExperienceLevel == experienceLevel);
            }

            // Apply search term filter last (least selective)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                if (term.Length > 0)
                {
                    query = query.Where(r => 
                        r.Title.ToLower().Contains(term) ||
                        (r.Description != null && r.Description.ToLower().Contains(term)) ||
                        (r.Department != null && r.Department.ToLower().Contains(term)) ||
                        (r.Location != null && r.Location.ToLower().Contains(term)) ||
                        (r.RequiredSkills != null && r.RequiredSkills.ToLower().Contains(term)));
                }
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetSearchCountAsync(string? searchTerm, string? status, string? department, string? priority, string? employmentType, string? experienceLevel, bool? isDraft)
        {
            var query = _context.Requisitions.AsQueryable();

            // Apply most selective filters first
            if (isDraft.HasValue)
            {
                query = query.Where(r => r.IsDraft == isDraft.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                query = query.Where(r => r.Priority == priority);
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(r => r.Department == department);
            }

            if (!string.IsNullOrWhiteSpace(employmentType))
            {
                query = query.Where(r => r.EmploymentType == employmentType);
            }

            if (!string.IsNullOrWhiteSpace(experienceLevel))
            {
                query = query.Where(r => r.ExperienceLevel == experienceLevel);
            }

            // Apply search term filter last (least selective)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                if (term.Length > 0)
                {
                    query = query.Where(r => 
                        r.Title.ToLower().Contains(term) ||
                        (r.Description != null && r.Description.ToLower().Contains(term)) ||
                        (r.Department != null && r.Department.ToLower().Contains(term)) ||
                        (r.Location != null && r.Location.ToLower().Contains(term)) ||
                        (r.RequiredSkills != null && r.RequiredSkills.ToLower().Contains(term)));
                }
            }

            return await query.CountAsync();
        }
    }
}
