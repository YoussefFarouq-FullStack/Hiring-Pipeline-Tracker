using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.DTOs;
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
        // Validate that the application exists
        var application = await _context.Applications.FindAsync(applicationId);
        if (application == null)
        {
            return NotFound($"Application with ID {applicationId} not found.");
        }

        return await _context.StageHistories
            .Where(sh => sh.ApplicationId == applicationId)
            .OrderBy(sh => sh.MovedAt)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<StageHistory>> PostStageHistory(CreateStageHistoryDto createDto)
    {
        // Validate that the application exists
        var application = await _context.Applications.FindAsync(createDto.ApplicationId);
        if (application == null)
        {
            return BadRequest($"Application with ID {createDto.ApplicationId} not found.");
        }

        // Check if this will be the first stage history and reset identity seed if needed
        if (!await _context.StageHistories.AnyAsync())
        {
            ResetIdentitySeed();
        }

        var stageHistory = new StageHistory
        {
            ApplicationId = createDto.ApplicationId,
            FromStage = createDto.FromStage,
            ToStage = createDto.ToStage,
            MovedBy = createDto.MovedBy,
            MovedAt = DateTime.UtcNow
        };

        _context.StageHistories.Add(stageHistory);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetStageHistory", new { id = stageHistory.StageHistoryId }, stageHistory);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStageHistory(int id, CreateStageHistoryDto updateDto)
    {
        var stageHistory = await _context.StageHistories.FindAsync(id);
        if (stageHistory == null)
            return NotFound();

        // Validate that the application exists
        var application = await _context.Applications.FindAsync(updateDto.ApplicationId);
        if (application == null)
        {
            return BadRequest($"Application with ID {updateDto.ApplicationId} not found.");
        }

        stageHistory.ApplicationId = updateDto.ApplicationId;
        stageHistory.FromStage = updateDto.FromStage;
        stageHistory.ToStage = updateDto.ToStage;
        stageHistory.MovedBy = updateDto.MovedBy;
        stageHistory.MovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
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

        // Check if this was the last stage history and reset identity seed if so
        if (!await _context.StageHistories.AnyAsync())
        {
            ResetIdentitySeed();
        }

        return NoContent();
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllStageHistories()
    {
        var stageHistories = await _context.StageHistories.ToListAsync();
        if (stageHistories.Any())
        {
            _context.StageHistories.RemoveRange(stageHistories);
            await _context.SaveChangesAsync();
            ResetIdentitySeed();
        }

        return NoContent();
    }

    private void ResetIdentitySeed()
    {
        // Reset the identity seed for StageHistories table using constant values (safe from SQL injection)
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('StageHistories', RESEED, 0)");
    }
}