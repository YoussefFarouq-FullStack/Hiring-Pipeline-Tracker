using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Services.Implementations
{
    public class RequisitionService : IRequisitionService
    {
        private readonly HiringPipelineDbContext _context;

        public RequisitionService(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Requisition>> GetAllAsync()
        {
            return await _context.Requisitions.ToListAsync();
        }

        public async Task<Requisition?> GetByIdAsync(int id)
        {
            return await _context.Requisitions.FindAsync(id);
        }

        public async Task<Requisition> CreateAsync(Requisition requisition)
        {
            _context.Requisitions.Add(requisition);
            await _context.SaveChangesAsync();
            return requisition;
        }

        public async Task<Requisition?> UpdateAsync(int id, Requisition requisition)
        {
            var existing = await _context.Requisitions.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(requisition);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Requisitions.FindAsync(id);
            if (existing == null) return false;

            _context.Requisitions.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AnyAsync()
        {
            return await _context.Requisitions.AnyAsync();
        }

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
    }
}
