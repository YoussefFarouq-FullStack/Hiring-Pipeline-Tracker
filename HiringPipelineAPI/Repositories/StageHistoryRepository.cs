using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.Repositories.Interfaces;

namespace HiringPipelineAPI.Repositories.Implementations
{
    public class StageHistoryRepository : IStageHistoryRepository
    {
        private readonly HiringPipelineDbContext _context;

        public StageHistoryRepository(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StageHistory>> GetByApplicationIdAsync(int applicationId) =>
            await _context.StageHistories
                .Where(h => h.ApplicationId == applicationId)
                .OrderByDescending(h => h.MovedAt)
                .ToListAsync();

        public async Task<StageHistory> AddAsync(StageHistory history)
        {
            _context.StageHistories.Add(history);
            await _context.SaveChangesAsync();
            return history;
        }

        public async Task<IEnumerable<StageHistory>> GetAllAsync() =>
            await _context.StageHistories
                .Include(sh => sh.Application)
                .ThenInclude(a => a.Candidate)
                .Include(sh => sh.Application)
                .ThenInclude(a => a.Requisition)
                .ToListAsync();

        public async Task<StageHistory?> GetByIdAsync(int id) =>
            await _context.StageHistories
                .Include(sh => sh.Application)
                .ThenInclude(a => a.Candidate)
                .Include(sh => sh.Application)
                .ThenInclude(a => a.Requisition)
                .FirstOrDefaultAsync(sh => sh.StageHistoryId == id);

        public async Task<StageHistory?> UpdateAsync(int id, StageHistory history)
        {
            var existing = await _context.StageHistories.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(history);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.StageHistories.FindAsync(id);
            if (existing == null) return false;

            _context.StageHistories.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AnyAsync() =>
            await _context.StageHistories.AnyAsync();

        public async Task DeleteAllAsync()
        {
            var stageHistories = await _context.StageHistories.ToListAsync();
            if (stageHistories.Any())
            {
                _context.StageHistories.RemoveRange(stageHistories);
                await _context.SaveChangesAsync();
            }
        }

        public void ResetIdentitySeed()
        {
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('StageHistories', RESEED, 0)");
        }
    }
}
