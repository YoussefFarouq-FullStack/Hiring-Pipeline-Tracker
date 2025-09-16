using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Entities;

namespace HiringPipelineAPI.Controllers;

/// <summary>
/// Manages job applications in the hiring pipeline
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // Require authentication for all application operations
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationApiService _applicationService;
    private readonly IAuditService _auditService;

    public ApplicationsController(IApplicationApiService applicationService, IAuditService auditService)
    {
        _applicationService = applicationService;
        _auditService = auditService;
    }

    /// <summary>
    /// Retrieves all job applications
    /// </summary>
    /// <returns>A list of all applications in the system</returns>
    /// <response code="200">Returns the list of applications</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Recruiter,Hiring Manager,Interviewer,Read-only")]
    [ProducesResponseType(typeof(IEnumerable<ApplicationDto>), 200)]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetApplications()
    {
        var applications = await _applicationService.GetAllAsync();
        return Ok(applications);
    }

    /// <summary>
    /// Retrieves a specific job application by ID
    /// </summary>
    /// <param name="id">The unique identifier of the application</param>
    /// <returns>The requested application with full details</returns>
    /// <response code="200">Returns the requested application</response>
    /// <response code="404">If the application was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Recruiter,Hiring Manager,Interviewer,Read-only")]
    [ProducesResponseType(typeof(ApplicationDetailDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApplicationDetailDto>> GetApplication(int id)
    {
        var application = await _applicationService.GetByIdAsync(id);
        return Ok(application);
    }

    /// <summary>
    /// Creates a new job application
    /// </summary>
    /// <param name="createDto">The application data to create</param>
    /// <returns>The newly created application</returns>
    /// <response code="201">Returns the newly created application</response>
    /// <response code="400">If the application data is invalid</response>
    /// <response code="404">If the candidate or requisition doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Recruiter")]
    [ProducesResponseType(typeof(ApplicationDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApplicationDto>> CreateApplication([FromBody] CreateApplicationDto createDto)
    {
        var createdApplication = await _applicationService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetApplication), new { id = createdApplication.ApplicationId }, createdApplication);
    }

    /// <summary>
    /// Updates an existing job application
    /// </summary>
    /// <param name="id">The unique identifier of the application to update</param>
    /// <param name="updateDto">The updated application data</param>
    /// <returns>No content on successful update</returns>
    /// <response code="204">If the application was successfully updated</response>
    /// <response code="400">If the update data is invalid</response>
    /// <response code="404">If the application was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recruiter,Hiring Manager")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateApplication(int id, [FromBody] UpdateApplicationDto updateDto)
    {
        // Get the current application to check for stage changes
        var existingApplication = await _applicationService.GetByIdAsync(id);
        var oldStage = existingApplication.CurrentStage;

        await _applicationService.UpdateAsync(id, updateDto);

        // Log the application update
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Unknown";
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";

        var changes = new List<string>();
        if (oldStage != updateDto.CurrentStage)
            changes.Add($"CurrentStage: {oldStage} → {updateDto.CurrentStage}");
        if (existingApplication.Status != updateDto.Status)
            changes.Add($"Status: {existingApplication.Status} → {updateDto.Status}");

        await _auditService.LogAsync(
            userId: userId,
            username: username,
            userRole: userRole,
            action: "Update Application",
            entity: "Application",
            entityId: id,
            changes: changes.Count > 0 ? string.Join("; ", changes) : "No changes detected",
            details: oldStage != updateDto.CurrentStage ? $"Stage changed from {oldStage} to {updateDto.CurrentStage}" : null,
            ipAddress: ipAddress,
            userAgent: userAgent,
            logType: AuditLogType.UserAction
        );

        return NoContent();
    }

    /// <summary>
    /// Deletes a job application
    /// </summary>
    /// <param name="id">The unique identifier of the application to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If the application was successfully deleted</response>
    /// <response code="404">If the application was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        await _applicationService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Changes the status of a job application
    /// </summary>
    /// <param name="id">The unique identifier of the application</param>
    /// <param name="statusDto">The new status information</param>
    /// <returns>Success message</returns>
    /// <response code="200">If the status was successfully changed</response>
    /// <response code="404">If the application was not found</response>
    /// <response code="400">If the status is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("{id}/change-status")]
    [Authorize(Roles = "Admin,Recruiter,Hiring Manager")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ChangeApplicationStatus(int id, [FromBody] ChangeApplicationStatusDto statusDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _applicationService.ChangeStatusAsync(id, statusDto);
            return Ok(new { message = "Application status changed successfully", newStatus = statusDto.Status });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Moves a candidate to a different stage in the pipeline
    /// </summary>
    /// <param name="id">The unique identifier of the application</param>
    /// <param name="stageDto">The stage movement information</param>
    /// <returns>Success message</returns>
    /// <response code="200">If the candidate was successfully moved to the stage</response>
    /// <response code="404">If the application was not found</response>
    /// <response code="400">If the stage movement is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("{id}/move-stage")]
    [Authorize(Roles = "Admin,Recruiter,Hiring Manager")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> MoveCandidateToStage(int id, [FromBody] MoveToStageDto stageDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _applicationService.MoveToStageAsync(id, stageDto);
            return Ok(new { message = "Candidate moved to stage successfully", newStage = stageDto.ToStage });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
