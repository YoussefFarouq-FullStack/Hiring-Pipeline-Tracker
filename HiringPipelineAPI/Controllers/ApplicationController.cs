using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.Data;
using HiringPipelineAPI.DTOs;


namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly HiringPipelineDbContext _context;

    public ApplicationsController(HiringPipelineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
    {
        return await _context.Applications
            .Include(a => a.Candidate)
            .Include(a => a.Requisition)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Application>> GetApplication(int id)
    {
        var application = await _context.Applications
            .Include(a => a.Candidate)
            .Include(a => a.Requisition)
            .FirstOrDefaultAsync(a => a.ApplicationId == id);

        if (application == null) return NotFound();
        return application;
    }

    [HttpPost]
    public async Task<ActionResult<Application>> CreateApplication([FromBody] ApplicationDto dto)
    {
        var application = new Application
        {
            CandidateId = dto.CandidateId,
            RequisitionId = dto.RequisitionId,
            CurrentStage = dto.CurrentStage,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetApplication), new { id = application.ApplicationId }, application);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateApplication(int id, [FromBody] ApplicationDto dto)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null) return NotFound();

        application.CandidateId = dto.CandidateId;
        application.RequisitionId = dto.RequisitionId;
        application.CurrentStage = dto.CurrentStage;
        application.Status = dto.Status;
        application.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var app = await _context.Applications.FindAsync(id);
        if (app == null) return NotFound();

        _context.Applications.Remove(app);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
