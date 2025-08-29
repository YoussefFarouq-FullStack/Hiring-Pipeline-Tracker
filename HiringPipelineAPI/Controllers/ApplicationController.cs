using Microsoft.AspNetCore.Mvc;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
    {
        var applications = await _applicationService.GetAllAsync();
        return Ok(applications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Application>> GetApplication(int id)
    {
        var application = await _applicationService.GetByIdAsync(id);
        if (application == null) return NotFound();
        return application;
    }

    [HttpPost]
    public async Task<ActionResult<Application>> CreateApplication([FromBody] CreateApplicationDto createDto)
    {
        // Only CandidateId & RequisitionId are required here
        if (!await _applicationService.CandidateExistsAsync(createDto.CandidateId))
            return BadRequest("Invalid CandidateId");

        if (!await _applicationService.RequisitionExistsAsync(createDto.RequisitionId))
            return BadRequest("Invalid RequisitionId");

        var application = new Application
        {
            CandidateId = createDto.CandidateId,
            RequisitionId = createDto.RequisitionId,
            CurrentStage = createDto.CurrentStage ?? "Applied",
            Status = createDto.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdApplication = await _applicationService.CreateAsync(application);
        return CreatedAtAction(nameof(GetApplication), new { id = createdApplication.ApplicationId }, createdApplication);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateApplication(int id, [FromBody] UpdateApplicationDto updateDto)
    {
        var existing = await _applicationService.GetByIdAsync(id);
        if (existing == null) return NotFound();

        if (updateDto.CurrentStage != null)
            existing.CurrentStage = updateDto.CurrentStage;
        if (updateDto.Status != null)
            existing.Status = updateDto.Status;

        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _applicationService.UpdateAsync(id, existing);
        if (updated == null) return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var deleted = await _applicationService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
