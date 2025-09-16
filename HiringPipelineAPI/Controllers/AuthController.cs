using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using HiringPipelineInfrastructure.Data;
using System.Security.Claims;
using HiringPipelineCore.Entities;

namespace HiringPipelineAPI.Controllers
{
    /// <summary>
    /// Authentication endpoints for user login, token refresh, and logout
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly HiringPipelineDbContext _context;
        private readonly IAuditService _auditService;

        public AuthController(IAuthService authService, HiringPipelineDbContext context, IAuditService auditService)
        {
            _authService = authService;
            _context = context;
            _auditService = auditService;
        }

        /// <summary>
        /// Authenticates a user and returns JWT access token and refresh token
        /// </summary>
        /// <param name="loginDto">User credentials</param>
        /// <returns>JWT access token, refresh token, and user information</returns>
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

            var ipAddress = GetClientIpAddress();
            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Log successful login
            await _auditService.LogAsync(
                result.User.Id,
                result.User.Username,
                result.User.Roles.FirstOrDefault() ?? "Unknown",
                "Login",
                "Authentication",
                null,
                null,
                $"User logged in from IP: {ipAddress}",
                ipAddress,
                Request.Headers["User-Agent"].FirstOrDefault(),
                AuditLogType.Authentication
            );

            return Ok(result);
        }

        /// <summary>
        /// Refreshes the access token using a valid refresh token
        /// </summary>
        /// <param name="refreshTokenDto">Refresh token</param>
        /// <returns>New JWT access token and refresh token</returns>
        /// <response code="200">Token refreshed successfully</response>
        /// <response code="401">Invalid or expired refresh token</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = GetClientIpAddress();
            var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken, ipAddress);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Logs out the user and revokes all refresh tokens
        /// </summary>
        /// <returns>Success message</returns>
        /// <response code="200">Logout successful</response>
        /// <response code="401">User not authenticated</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var ipAddress = GetClientIpAddress();
            await _authService.RevokeAllRefreshTokensForUserAsync(userId, ipAddress);

            // Log logout
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
            
            await _auditService.LogAsync(
                userId,
                username,
                userRole,
                "Logout",
                "Authentication",
                null,
                null,
                $"User logged out from IP: {ipAddress}",
                ipAddress,
                Request.Headers["User-Agent"].FirstOrDefault(),
                AuditLogType.Authentication
            );

            return Ok(new { message = "Logout successful" });
        }

        /// <summary>
        /// Revokes a specific refresh token
        /// </summary>
        /// <param name="refreshTokenDto">Refresh token to revoke</param>
        /// <returns>Success message</returns>
        /// <response code="200">Token revoked successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Token not found</response>
        [HttpPost("revoke")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> RevokeToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = GetClientIpAddress();
            var success = await _authService.RevokeRefreshTokenAsync(refreshTokenDto.RefreshToken, ipAddress);

            if (!success)
            {
                return NotFound(new { message = "Token not found or already revoked" });
            }

            return Ok(new { message = "Token revoked successfully" });
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

        private string? GetClientIpAddress()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
