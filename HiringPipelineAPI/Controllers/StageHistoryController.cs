using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StageHistoryController : ControllerBase
{
    private readonly HiringPipelineDbContext _context;

    public StageHistoryController(HiringPipelineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StageHistory>>> GetStageHistories()
    {
        return await _context.StageHistories
            .Include(sh => sh.Application)
            .ThenInclude(a => a.Candidate)
            .Include(sh => sh.Application)
            .ThenInclude(a => a.Requisition)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StageHistory>> GetStageHistory(int id)
    {
        var stageHistory = await _context.StageHistories
            .Include(sh => sh.Application)
            .ThenInclude(a => a.Candidate)
            .Include(sh => sh.Application)
            .ThenInclude(a => a.Requisition)
            .FirstOrDefaultAsync(sh => sh.StageHistoryId == id);

        if (stageHistory == null)
        {
            return NotFound();
        }

        return stageHistory;
    }

    [HttpGet("application/{applicationId}")]
    public async Task<ActionResult<IEnumerable<StageHistory>>> GetApplicationHistory(int applicationId)
    {
        return await _context.StageHistories
            .Where(sh => sh.ApplicationId == applicationId)
            .OrderBy(sh => sh.MovedAt)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<StageHistory>> PostStageHistory(StageHistory stageHistory)
    {
        _context.StageHistories.Add(stageHistory);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetStageHistory", new { id = stageHistory.StageHistoryId }, stageHistory);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStageHistory(int id)
    {
        var stageHistory = await _context.StageHistories.FindAsync(id);
        if (stageHistory == null)
        {
            return NotFound();
        }

        _context.StageHistories.Remove(stageHistory);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}