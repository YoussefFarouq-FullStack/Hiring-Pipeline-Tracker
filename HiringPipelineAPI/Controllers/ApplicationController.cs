using Microsoft.AspNetCore.Mvc;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Controllers;

/// <summary>
/// Manages job applications in the hiring pipeline
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationApiService _applicationService;

    public ApplicationsController(IApplicationApiService applicationService)
    {
        _applicationService = applicationService;
    }

    /// <summary>
    /// Retrieves all job applications
    /// </summary>
    /// <returns>A list of all applications in the system</returns>
    /// <response code="200">Returns the list of applications</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
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
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateApplication(int id, [FromBody] UpdateApplicationDto updateDto)
    {
        await _applicationService.UpdateAsync(id, updateDto);
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
}
