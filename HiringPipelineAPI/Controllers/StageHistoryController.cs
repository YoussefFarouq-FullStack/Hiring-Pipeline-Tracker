using Microsoft.AspNetCore.Mvc;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Controllers;

/// <summary>
/// Manages stage history for job applications in the hiring pipeline
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StageHistoryController : ControllerBase
{
    private readonly IStageHistoryApiService _stageHistoryService;

    public StageHistoryController(IStageHistoryApiService stageHistoryService)
    {
        _stageHistoryService = stageHistoryService;
    }

    /// <summary>
    /// Retrieves all stage history entries
    /// </summary>
    /// <returns>A list of all stage history entries in the system</returns>
    /// <response code="200">Returns the list of stage history entries</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StageHistoryDto>), 200)]
    public async Task<ActionResult<IEnumerable<StageHistoryDto>>> GetStageHistories()
    {
        var stageHistories = await _stageHistoryService.GetAllAsync();
        return Ok(stageHistories);
    }

    /// <summary>
    /// Retrieves a specific stage history entry by ID
    /// </summary>
    /// <param name="id">The unique identifier of the stage history entry</param>
    /// <returns>The requested stage history entry with full details</returns>
    /// <response code="200">Returns the requested stage history entry</response>
    /// <response code="404">If the stage history entry was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StageHistoryDetailDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<StageHistoryDetailDto>> GetStageHistory(int id)
    {
        var stageHistory = await _stageHistoryService.GetByIdAsync(id);
        return Ok(stageHistory);
    }

    /// <summary>
    /// Retrieves stage history for a specific application
    /// </summary>
    /// <param name="applicationId">The unique identifier of the application</param>
    /// <returns>A list of stage history entries for the specified application</returns>
    /// <response code="200">Returns the stage history for the application</response>
    /// <response code="404">If the application was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("application/{applicationId}")]
    [ProducesResponseType(typeof(IEnumerable<StageHistoryDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<StageHistoryDto>>> GetStageHistoriesByApplication(int applicationId)
    {
        var stageHistories = await _stageHistoryService.GetByApplicationIdAsync(applicationId);
        return Ok(stageHistories);
    }

    /// <summary>
    /// Creates a new stage history entry
    /// </summary>
    /// <param name="createDto">The stage history data to create</param>
    /// <returns>The newly created stage history entry</returns>
    /// <response code="201">Returns the newly created stage history entry</response>
    /// <response code="400">If the stage history data is invalid</response>
    /// <response code="404">If the application doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(StageHistoryDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<StageHistoryDto>> CreateStageHistory([FromBody] CreateStageHistoryDto createDto)
    {
        var createdStageHistory = await _stageHistoryService.AddStageAsync(createDto);
        return CreatedAtAction(nameof(GetStageHistory), new { id = createdStageHistory.StageHistoryId }, createdStageHistory);
    }

    /// <summary>
    /// Updates an existing stage history entry
    /// </summary>
    /// <param name="id">The unique identifier of the stage history entry to update</param>
    /// <param name="updateDto">The updated stage history data</param>
    /// <returns>No content on successful update</returns>
    /// <response code="204">If the stage history entry was successfully updated</response>
    /// <response code="400">If the update data is invalid</response>
    /// <response code="404">If the stage history entry was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateStageHistory(int id, [FromBody] CreateStageHistoryDto updateDto)
    {
        await _stageHistoryService.UpdateAsync(id, updateDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a stage history entry
    /// </summary>
    /// <param name="id">The unique identifier of the stage history entry to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If the stage history entry was successfully deleted</response>
    /// <response code="404">If the stage history entry was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteStageHistory(int id)
    {
        await _stageHistoryService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Deletes all stage history entries and resets identity seed
    /// </summary>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If all stage history entries were successfully deleted</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("all")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteAllStageHistories()
    {
        await _stageHistoryService.DeleteAllAsync();
        _stageHistoryService.ResetIdentitySeed();
        return NoContent();
    }
}
