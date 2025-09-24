using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HiringPipelineAPI.Services.Interfaces;
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
    [Authorize(Roles = "Admin,Recruiter,Hiring Manager,Interviewer,Read-only")]
    [ProducesResponseType(typeof(IEnumerable<CandidateDto>), 200)]
    public async Task<ActionResult<IEnumerable<CandidateDto>>> GetCandidates()
    {
        var candidates = await _candidateService.GetAllAsync();
        return Ok(candidates);
    }

    /// <summary>
    /// Searches candidates with filtering and pagination
    /// </summary>
    /// <param name="searchTerm">Search term for name, email, or skills</param>
    /// <param name="status">Filter by candidate status</param>
    /// <param name="requisitionId">Filter by requisition ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Paginated search results</returns>
    /// <response code="200">Returns the search results</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("search")]
    [Authorize(Roles = "Admin,Recruiter,Hiring Manager,Interviewer,Read-only")]
    [ProducesResponseType(typeof(SearchResponseDto<CandidateDto>), 200)]
    public async Task<ActionResult<SearchResponseDto<CandidateDto>>> SearchCandidates(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null,
        [FromQuery] int? requisitionId = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var result = await _candidateService.SearchAsync(searchTerm, status, requisitionId, skip, take);
        return Ok(result);
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
    [Authorize(Roles = "Admin,Recruiter,Hiring Manager,Interviewer,Read-only")]
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
    [Authorize(Roles = "Admin,Recruiter")]
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
    [Authorize(Roles = "Admin,Recruiter")]
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

    /// <summary>
    /// Uploads a resume file for a candidate
    /// </summary>
    /// <param name="id">The unique identifier of the candidate</param>
    /// <param name="file">The resume file to upload</param>
    /// <returns>Success message with file information</returns>
    /// <response code="200">If the resume was successfully uploaded</response>
    /// <response code="404">If the candidate was not found</response>
    /// <response code="400">If the file is invalid or too large</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("{id}/upload-resume")]
    [Authorize(Roles = "Admin,Recruiter")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UploadResume(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file provided" });
        }

        if (file.Length > 10 * 1024 * 1024) // 10MB limit
        {
            return BadRequest(new { message = "File size exceeds 10MB limit" });
        }

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest(new { message = "Only PDF, DOC, and DOCX files are allowed" });
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileContent = memoryStream.ToArray();
            
            var result = await _candidateService.UploadResumeAsync(id, file.FileName, fileContent);
            return Ok(new { message = "Resume uploaded successfully", fileName = result.FileName, fileSize = result.FileSize });
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
    /// Adds skills/tags to a candidate
    /// </summary>
    /// <param name="id">The unique identifier of the candidate</param>
    /// <param name="skills">List of skills to add</param>
    /// <returns>Success message</returns>
    /// <response code="200">If the skills were successfully added</response>
    /// <response code="404">If the candidate was not found</response>
    /// <response code="400">If the skills data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("{id}/add-skills")]
    [Authorize(Roles = "Admin,Recruiter")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddSkills(int id, [FromBody] List<string> skills)
    {
        if (skills == null || !skills.Any())
        {
            return BadRequest(new { message = "Skills list cannot be empty" });
        }

        try
        {
            await _candidateService.AddSkillsAsync(id, skills);
            return Ok(new { message = "Skills added successfully", skillsCount = skills.Count });
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
    /// Archives a candidate (soft delete)
    /// </summary>
    /// <param name="id">The unique identifier of the candidate to archive</param>
    /// <returns>Success message</returns>
    /// <response code="200">If the candidate was successfully archived</response>
    /// <response code="404">If the candidate was not found</response>
    /// <response code="400">If the candidate is already archived</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("{id}/archive")]
    [Authorize(Roles = "Admin,Recruiter")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ArchiveCandidate(int id)
    {
        try
        {
            await _candidateService.ArchiveAsync(id);
            return Ok(new { message = "Candidate archived successfully" });
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
