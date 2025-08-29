using Microsoft.AspNetCore.Mvc;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidateDto>>> GetCandidates()
    {
        var candidates = await _candidateService.GetAllAsync();
        return Ok(candidates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CandidateDetailDto>> GetCandidate(int id)
    {
        var candidate = await _candidateService.GetByIdAsync(id);
        return Ok(candidate);
    }

    [HttpPost]
    public async Task<ActionResult<CandidateDto>> CreateCandidate([FromBody] CreateCandidateDto createDto)
    {
        var createdCandidate = await _candidateService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetCandidate), new { id = createdCandidate.CandidateId }, createdCandidate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCandidate(int id, [FromBody] UpdateCandidateDto updateDto)
    {
        var updated = await _candidateService.UpdateAsync(id, updateDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidate(int id)
    {
        await _candidateService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAllCandidates()
    {
        await _candidateService.DeleteAllAsync();
        _candidateService.ResetIdentitySeed();
        return NoContent();
    }
}
