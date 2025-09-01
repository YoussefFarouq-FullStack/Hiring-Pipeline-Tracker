using Microsoft.AspNetCore.Mvc;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Controllers;

/// <summary>
/// Manages job requisitions in the hiring pipeline
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RequisitionsController : ControllerBase
{
    private readonly IRequisitionService _requisitionService;

    public RequisitionsController(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    /// <summary>
    /// Retrieves all job requisitions
    /// </summary>
    /// <returns>A list of all requisitions in the system</returns>
    /// <response code="200">Returns the list of requisitions</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RequisitionDto>), 200)]
    public async Task<ActionResult<IEnumerable<RequisitionDto>>> GetRequisitions()
    {
        var requisitions = await _requisitionService.GetAllAsync();
        return Ok(requisitions);
    }

    /// <summary>
    /// Retrieves a specific job requisition by ID
    /// </summary>
    /// <param name="id">The unique identifier of the requisition</param>
    /// <returns>The requested requisition with full details</returns>
    /// <response code="200">Returns the requested requisition</response>
    /// <response code="404">If the requisition was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RequisitionDetailDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RequisitionDetailDto>> GetRequisition(int id)
    {
        var requisition = await _requisitionService.GetByIdAsync(id);
        return Ok(requisition);
    }

    /// <summary>
    /// Creates a new job requisition
    /// </summary>
    /// <param name="createDto">The requisition data to create</param>
    /// <returns>The newly created requisition</returns>
    /// <response code="201">Returns the newly created requisition</response>
    /// <response code="400">If the requisition data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(RequisitionDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<RequisitionDto>> CreateRequisition([FromBody] CreateRequisitionDto createDto)
    {
        var createdRequisition = await _requisitionService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetRequisition), new { id = createdRequisition.RequisitionId }, createdRequisition);
    }

    /// <summary>
    /// Updates an existing job requisition
    /// </summary>
    /// <param name="id">The unique identifier of the requisition to update</param>
    /// <param name="updateDto">The updated requisition data</param>
    /// <returns>No content on successful update</returns>
    /// <response code="204">If the requisition was successfully updated</response>
    /// <response code="400">If the update data is invalid</response>
    /// <response code="404">If the requisition was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRequisition(int id, [FromBody] UpdateRequisitionDto updateDto)
    {
        await _requisitionService.UpdateAsync(id, updateDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a job requisition
    /// </summary>
    /// <param name="id">The unique identifier of the requisition to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If the requisition was successfully deleted</response>
    /// <response code="404">If the requisition was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteRequisition(int id)
    {
        await _requisitionService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Deletes all requisitions and resets identity seed
    /// </summary>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If all requisitions were successfully deleted</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("all")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteAllRequisitions()
    {
        await _requisitionService.DeleteAllAsync();
        _requisitionService.ResetIdentitySeed();
        return NoContent();
    }
}
