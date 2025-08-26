using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidateController : ControllerBase
{
    private readonly HiringPipelineDbContext _context;

    public CandidateController(HiringPipelineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
    {
        return await _context.Candidates.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Candidate>> GetCandidate(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);

        if (candidate == null)
            return NotFound();

        return candidate;
    }

    [HttpPost]
    public async Task<ActionResult<Candidate>> CreateCandidate(CreateCandidateDto createDto)
    {
        // Check if this will be the first candidate and reset identity seed if needed
        if (!await _context.Candidates.AnyAsync())
        {
            ResetIdentitySeed();
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

        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCandidate), new { id = candidate.CandidateId }, candidate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCandidate(int id, UpdateCandidateDto updateDto)
    {
        var candidate = await _context.Candidates.FindAsync(id);
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

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidate(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null)
            return NotFound();

        _context.Candidates.Remove(candidate);
        await _context.SaveChangesAsync();

        // Check if this was the last candidate and reset identity seed if so
        if (!await _context.Candidates.AnyAsync())
        {
            ResetIdentitySeed();
        }

        return NoContent();
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllCandidates()
    {
        var candidates = await _context.Candidates.ToListAsync();
        if (candidates.Any())
        {
            _context.Candidates.RemoveRange(candidates);
            await _context.SaveChangesAsync();
            ResetIdentitySeed();
        }

        return NoContent();
    }

    private void ResetIdentitySeed()
    {
        // Reset the identity seed for Candidates table using constant values (safe from SQL injection)
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Candidates', RESEED, 0)");
    }
}
