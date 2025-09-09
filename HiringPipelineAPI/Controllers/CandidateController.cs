using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Controllers;

/// <summary>
/// Manages candidates in the hiring pipeline
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // Require authentication for all candidate operations
public class CandidatesController : ControllerBase
{
    private readonly ICandidateApiService _candidateService;

    public CandidatesController(ICandidateApiService candidateService)
    {
        _candidateService = candidateService;
    }

    /// <summary>
    /// Retrieves all candidates
    /// </summary>
    /// <returns>A list of all candidates in the system</returns>
    /// <response code="200">Returns the list of candidates</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CandidateDto>), 200)]
    public async Task<ActionResult<IEnumerable<CandidateDto>>> GetCandidates()
    {
        var candidates = await _candidateService.GetAllAsync();
        return Ok(candidates);
    }

    /// <summary>
    /// Retrieves a specific candidate by ID
    /// </summary>
    /// <param name="id">The unique identifier of the candidate</param>
    /// <returns>The requested candidate with full details</returns>
    /// <response code="200">Returns the requested candidate</response>
    /// <response code="404">If the candidate was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CandidateDetailDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CandidateDetailDto>> GetCandidate(int id)
    {
        var candidate = await _candidateService.GetByIdAsync(id);
        return Ok(candidate);
    }

    /// <summary>
    /// Creates a new candidate
    /// </summary>
    /// <param name="createDto">The candidate data to create, including optional description and resume file information</param>
    /// <returns>The newly created candidate</returns>
    /// <response code="201">Returns the newly created candidate</response>
    /// <response code="400">If the candidate data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(CandidateDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<CandidateDto>> CreateCandidate([FromBody] CreateCandidateDto createDto)
    {
        var createdCandidate = await _candidateService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetCandidate), new { id = createdCandidate.CandidateId }, createdCandidate);
    }

    /// <summary>
    /// Updates an existing candidate
    /// </summary>
    /// <param name="id">The unique identifier of the candidate to update</param>
    /// <param name="updateDto">The updated candidate data, including optional description and resume file information</param>
    /// <returns>No content on successful update</returns>
    /// <response code="204">If the candidate was successfully updated</response>
    /// <response code="400">If the update data is invalid</response>
    /// <response code="404">If the candidate was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateCandidate(int id, [FromBody] UpdateCandidateDto updateDto)
    {
        await _candidateService.UpdateAsync(id, updateDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a candidate
    /// </summary>
    /// <param name="id">The unique identifier of the candidate to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If the candidate was successfully deleted</response>
    /// <response code="404">If the candidate was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can delete candidates
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteCandidate(int id)
    {
        await _candidateService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Deletes all candidates and resets identity seed
    /// </summary>
    /// <returns>No content on successful deletion</returns>
    /// <response code="204">If all candidates were successfully deleted</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("all")]
    [Authorize(Roles = "Admin")] // Only admins can delete all candidates
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteAllCandidates()
    {
        await _candidateService.DeleteAllAsync();
        _candidateService.ResetIdentitySeed();
        return NoContent();
    }
}
