using Microsoft.AspNetCore.Mvc;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequisitionsController : ControllerBase
{
    private readonly IRequisitionService _requisitionService;

    public RequisitionsController(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RequisitionDto>>> GetRequisitions()
    {
        var requisitions = await _requisitionService.GetAllAsync();
        return Ok(requisitions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RequisitionDetailDto>> GetRequisition(int id)
    {
        var requisition = await _requisitionService.GetByIdAsync(id);
        return Ok(requisition);
    }

    [HttpPost]
    public async Task<ActionResult<RequisitionDto>> CreateRequisition([FromBody] CreateRequisitionDto createDto)
    {
        var createdRequisition = await _requisitionService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetRequisition), new { id = createdRequisition.RequisitionId }, createdRequisition);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRequisition(int id, [FromBody] UpdateRequisitionDto updateDto)
    {
        var updated = await _requisitionService.UpdateAsync(id, updateDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRequisition(int id)
    {
        await _requisitionService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAllRequisitions()
    {
        await _requisitionService.DeleteAllAsync();
        _requisitionService.ResetIdentitySeed();
        return NoContent();
    }
}
