using HiringPipelineAPI.Data;
using HiringPipelineAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly HiringPipelineDbContext _context;

    public ApplicationController(HiringPipelineDbContext context)
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

        if (application == null)
            return NotFound();

        return application;
    }

    [HttpPost]
    public async Task<ActionResult<Application>> CreateApplication(Application application)
    {
        // Check if this will be the first application and reset identity seed if needed
        if (!await _context.Applications.AnyAsync())
        {
            ResetIdentitySeed();
        }

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetApplication), new { id = application.ApplicationId }, application);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateApplication(int id, Application application)
    {
        if (id != application.ApplicationId)
            return BadRequest();

        _context.Entry(application).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null)
            return NotFound();

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();

        // Check if this was the last application and reset identity seed if so
        if (!await _context.Applications.AnyAsync())
        {
            ResetIdentitySeed();
        }

        return NoContent();
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllApplications()
    {
        var applications = await _context.Applications.ToListAsync();
        if (applications.Any())
        {
            _context.Applications.RemoveRange(applications);
            await _context.SaveChangesAsync();
            ResetIdentitySeed();
        }

        return NoContent();
    }

    private void ResetIdentitySeed()
    {
        // Reset the identity seed for Applications table
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Applications', RESEED, 0)");
    }
}
