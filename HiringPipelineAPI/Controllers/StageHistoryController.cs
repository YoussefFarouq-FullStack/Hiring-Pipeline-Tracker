using Microsoft.AspNetCore.Mvc;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StageHistoryController : ControllerBase
{
    private readonly IStageHistoryService _stageHistoryService;

    public StageHistoryController(IStageHistoryService stageHistoryService)
    {
        _stageHistoryService = stageHistoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StageHistoryDto>>> GetStageHistories()
    {
        var stageHistories = await _stageHistoryService.GetAllAsync();
        return Ok(stageHistories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StageHistoryDetailDto>> GetStageHistory(int id)
    {
        var stageHistory = await _stageHistoryService.GetByIdAsync(id);
        return Ok(stageHistory);
    }

    [HttpGet("application/{applicationId}")]
    public async Task<ActionResult<IEnumerable<StageHistoryDto>>> GetStageHistoriesByApplication(int applicationId)
    {
        var stageHistories = await _stageHistoryService.GetByApplicationIdAsync(applicationId);
        return Ok(stageHistories);
    }

    [HttpPost]
    public async Task<ActionResult<StageHistoryDto>> CreateStageHistory([FromBody] CreateStageHistoryDto createDto)
    {
        var createdStageHistory = await _stageHistoryService.AddStageAsync(createDto);
        return CreatedAtAction(nameof(GetStageHistory), new { id = createdStageHistory.StageHistoryId }, createdStageHistory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStageHistory(int id, [FromBody] CreateStageHistoryDto updateDto)
    {
        var updated = await _stageHistoryService.UpdateAsync(id, updateDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStageHistory(int id)
    {
        await _stageHistoryService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAllStageHistories()
    {
        await _stageHistoryService.DeleteAllAsync();
        _stageHistoryService.ResetIdentitySeed();
        return NoContent();
    }
}