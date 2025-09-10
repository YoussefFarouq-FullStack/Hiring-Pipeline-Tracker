using Microsoft.AspNetCore.Mvc;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using HiringPipelineInfrastructure.Data;

namespace HiringPipelineAPI.Controllers
{
    /// <summary>
    /// Authentication endpoints for user login
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

    private readonly HiringPipelineDbContext _context;

    public AuthController(IAuthService authService, HiringPipelineDbContext context)
    {
        _authService = authService;
        _context = context;
    }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="loginDto">User credentials</param>
        /// <returns>JWT token and user information</returns>
        /// <response code="200">Login successful</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

        return Ok(result);
    }

    /// <summary>
    /// Debug endpoint to check if users exist in database (temporary)
    /// </summary>
    [HttpGet("debug/users")]
    public async Task<ActionResult> GetUsers()
    {
        // This is a temporary debug endpoint - remove in production
        var users = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Select(u => new { 
                u.Id, 
                u.Username, 
                u.Email,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                PasswordHashLength = u.PasswordHash.Length,
                u.CreatedAt 
            }).ToListAsync();
        
        return Ok(new { 
            message = "Debug endpoint - users in database",
            userCount = users.Count,
            users = users
        });
    }
}
}
