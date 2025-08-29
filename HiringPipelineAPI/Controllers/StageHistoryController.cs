using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StageHistoryController : ControllerBase
{
    private readonly IStageHistoryService _stageHistoryService;
    private readonly IApplicationService _applicationService;

    public StageHistoryController(IStageHistoryService stageHistoryService, IApplicationService applicationService)
    {
        _stageHistoryService = stageHistoryService;
        _applicationService = applicationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StageHistory>>> GetStageHistories()
    {
        var stageHistories = await _stageHistoryService.GetAllAsync();
        return Ok(stageHistories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StageHistory>> GetStageHistory(int id)
    {
        var stageHistory = await _stageHistoryService.GetByIdAsync(id);

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
        var application = await _applicationService.GetByIdAsync(applicationId);
        if (application == null)
        {
            return NotFound($"Application with ID {applicationId} not found.");
        }

        var stageHistories = await _stageHistoryService.GetByApplicationIdAsync(applicationId);
        return Ok(stageHistories);
    }

    [HttpPost]
    public async Task<ActionResult<StageHistory>> PostStageHistory(CreateStageHistoryDto createDto)
    {
        // Validate that the application exists
        var application = await _applicationService.GetByIdAsync(createDto.ApplicationId);
        if (application == null)
        {
            return BadRequest($"Application with ID {createDto.ApplicationId} not found.");
        }

        // Check if this will be the first stage history and reset identity seed if needed
        if (!await _stageHistoryService.AnyAsync())
        {
            _stageHistoryService.ResetIdentitySeed();
        }

        var stageHistory = new StageHistory
        {
            ApplicationId = createDto.ApplicationId,
            FromStage = createDto.FromStage,
            ToStage = createDto.ToStage,
            MovedBy = createDto.MovedBy,
            MovedAt = DateTime.UtcNow
        };

        var createdStageHistory = await _stageHistoryService.AddStageAsync(stageHistory);
        return CreatedAtAction("GetStageHistory", new { id = createdStageHistory.StageHistoryId }, createdStageHistory);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStageHistory(int id, CreateStageHistoryDto updateDto)
    {
        var stageHistory = await _stageHistoryService.GetByIdAsync(id);
        if (stageHistory == null)
            return NotFound();

        // Validate that the application exists
        var application = await _applicationService.GetByIdAsync(updateDto.ApplicationId);
        if (application == null)
        {
            return BadRequest($"Application with ID {updateDto.ApplicationId} not found.");
        }

        stageHistory.ApplicationId = updateDto.ApplicationId;
        stageHistory.FromStage = updateDto.FromStage;
        stageHistory.ToStage = updateDto.ToStage;
        stageHistory.MovedBy = updateDto.MovedBy;
        stageHistory.MovedAt = DateTime.UtcNow;

        var updated = await _stageHistoryService.UpdateAsync(id, stageHistory);
        if (updated == null) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStageHistory(int id)
    {
        var deleted = await _stageHistoryService.DeleteAsync(id);
        if (!deleted) return NotFound();

        // Check if this was the last stage history and reset identity seed if so
        if (!await _stageHistoryService.AnyAsync())
        {
            _stageHistoryService.ResetIdentitySeed();
        }

        return NoContent();
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllStageHistories()
    {
        await _stageHistoryService.DeleteAllAsync();
        _stageHistoryService.ResetIdentitySeed();
        return NoContent();
    }
}