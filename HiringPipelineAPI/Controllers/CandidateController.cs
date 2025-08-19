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

        return NoContent();
    }
}
