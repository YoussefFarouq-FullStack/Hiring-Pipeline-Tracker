using Microsoft.AspNetCore.Mvc;

namespace HiringPipelineAPI.Controllers;

/// <summary>
/// Health check and system status endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>API status message</returns>
    /// <response code="200">API is running successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult Get()
    {
        return Ok("API is running");
    }
}
