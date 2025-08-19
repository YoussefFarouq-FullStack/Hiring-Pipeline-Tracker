using Microsoft.AspNetCore.Mvc;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new { 
            status = "Healthy", 
            timestamp = DateTime.UtcNow,
            service = "HiringPipelineAPI",
            version = "1.0.0"
        });
    }
}