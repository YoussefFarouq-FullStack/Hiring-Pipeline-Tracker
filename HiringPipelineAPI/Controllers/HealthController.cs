using Microsoft.AspNetCore.Mvc;
using HiringPipelineAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly HiringPipelineDbContext _context;

    public HealthController(HiringPipelineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("API is running");
    }

    [HttpPost("reset-identity-seeds")]
    public IActionResult ResetAllIdentitySeeds()
    {
        try
        {
            // Reset identity seeds for all tables
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Candidates', RESEED, 0)");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Requisitions', RESEED, 0)");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Applications', RESEED, 0)");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('StageHistories', RESEED, 0)");

            return Ok("All identity seeds have been reset to 0");
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to reset identity seeds: {ex.Message}");
        }
    }
}