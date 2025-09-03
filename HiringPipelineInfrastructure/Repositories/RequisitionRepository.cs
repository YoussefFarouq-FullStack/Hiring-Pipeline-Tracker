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
    }
}
