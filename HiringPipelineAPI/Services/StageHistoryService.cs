using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Services.Implementations
{
    public class StageHistoryService : IStageHistoryService
    {
        private readonly HiringPipelineDbContext _context;

        public StageHistoryService(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StageHistory>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.StageHistories
                .Where(h => h.ApplicationId == applicationId)
                .OrderByDescending(h => h.MovedAt)
                .ToListAsync();
        }

        public async Task<StageHistory> AddStageAsync(StageHistory history)
        {
            _context.StageHistories.Add(history);
            await _context.SaveChangesAsync();
            return history;
        }

        public async Task<IEnumerable<StageHistory>> GetAllAsync()
        {
            return await _context.StageHistories
                .Include(sh => sh.Application)
                .ThenInclude(a => a.Candidate)
                .Include(sh => sh.Application)
                .ThenInclude(a => a.Requisition)
                .ToListAsync();
        }

        public async Task<StageHistory?> GetByIdAsync(int id)
        {
            return await _context.StageHistories
                .Include(sh => sh.Application)
                .ThenInclude(a => a.Candidate)
                .Include(sh => sh.Application)
                .ThenInclude(a => a.Requisition)
                .FirstOrDefaultAsync(sh => sh.StageHistoryId == id);
        }

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

        public async Task<bool> AnyAsync()
        {
            return await _context.StageHistories.AnyAsync();
        }

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
