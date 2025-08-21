using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
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
    public async Task<ActionResult<Candidate>> CreateCandidate(Candidate candidate)
    {
        // Check if this will be the first candidate and reset identity seed if needed
        if (!await _context.Candidates.AnyAsync())
        {
            ResetIdentitySeed();
        }

        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCandidate), new { id = candidate.CandidateId }, candidate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCandidate(int id, Candidate candidate)
    {
        if (id != candidate.CandidateId)
            return BadRequest();

        _context.Entry(candidate).State = EntityState.Modified;
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
        // Reset the identity seed for Candidates table
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Candidates', RESEED, 0)");
    }
}
