using HiringPipelineAPI.Models;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidateController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidateController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
    {
        var candidates = await _candidateService.GetAllAsync();
        return Ok(candidates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Candidate>> GetCandidate(int id)
    {
        var candidate = await _candidateService.GetByIdAsync(id);

        if (candidate == null)
            return NotFound();

        return candidate;
    }

    [HttpPost]
    public async Task<ActionResult<Candidate>> CreateCandidate(CreateCandidateDto createDto)
    {
        // Check if this will be the first candidate and reset identity seed if needed
        if (!await _candidateService.AnyAsync())
        {
            _candidateService.ResetIdentitySeed();
        }

        var candidate = new Candidate
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            Phone = createDto.Phone,
            LinkedInUrl = createDto.LinkedInUrl,
            Source = createDto.Source,
            Status = "Applied",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdCandidate = await _candidateService.CreateAsync(candidate);
        return CreatedAtAction(nameof(GetCandidate), new { id = createdCandidate.CandidateId }, createdCandidate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCandidate(int id, UpdateCandidateDto updateDto)
    {
        var candidate = await _candidateService.GetByIdAsync(id);
        if (candidate == null)
            return NotFound();

        if (updateDto.FirstName != null)
            candidate.FirstName = updateDto.FirstName;
        if (updateDto.LastName != null)
            candidate.LastName = updateDto.LastName;
        if (updateDto.Email != null)
            candidate.Email = updateDto.Email;
        if (updateDto.Phone != null)
            candidate.Phone = updateDto.Phone;
        if (updateDto.LinkedInUrl != null)
            candidate.LinkedInUrl = updateDto.LinkedInUrl;
        if (updateDto.Source != null)
            candidate.Source = updateDto.Source;
        if (updateDto.Status != null)
            candidate.Status = updateDto.Status;

        candidate.UpdatedAt = DateTime.UtcNow;

        var updated = await _candidateService.UpdateAsync(id, candidate);
        if (updated == null) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidate(int id)
    {
        var deleted = await _candidateService.DeleteAsync(id);
        if (!deleted) return NotFound();

        // Check if this was the last candidate and reset identity seed if so
        if (!await _candidateService.AnyAsync())
        {
            _candidateService.ResetIdentitySeed();
        }

        return NoContent();
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllCandidates()
    {
        await _candidateService.DeleteAllAsync();
        _candidateService.ResetIdentitySeed();
        return NoContent();
    }
}
