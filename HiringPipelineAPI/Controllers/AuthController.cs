using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineInfrastructure.Data;

namespace HiringPipelineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuditService _auditService;
        private readonly HiringPipelineDbContext _context;

        public AuthController(IAuthService authService, IAuditService auditService, HiringPipelineDbContext context)
        {
            _authService = authService;
            _auditService = auditService;
            _context = context;
        }

        // User login -> issues JWT + refresh token
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ip = GetClientIp();
            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized(new { message = "Invalid username or password" });

            // Audit log
            await _auditService.LogAsync(
                result.User.Id,
                result.User.Username,
                result.User.Roles.FirstOrDefault() ?? "Unknown",
                "Login",
                "Authentication",
                null,
                null,
                $"User logged in from IP: {ip}",
                ip,
                Request.Headers["User-Agent"].FirstOrDefault(),
                AuditLogType.Authentication
            );

            return Ok(result);
        }

        // Refresh access token using refresh token (with rotation)
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ip = GetClientIp();

            var result = await _authService.RefreshTokenAsync(dto.RefreshToken, ip);

            if (result == null)
                return Unauthorized(new { message = "Invalid or expired refresh token" });

            return Ok(result);
        }

        // Revoke a specific refresh token
        [HttpPost("revoke")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Revoke([FromBody] RefreshTokenDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ip = GetClientIp();
            var success = await _authService.RevokeRefreshTokenAsync(dto.RefreshToken, ip);

            if (!success)
                return NotFound(new { message = "Token not found or already revoked" });

            return Ok(new { message = "Token revoked successfully" });
        }

        // Logout -> revoke ALL refresh tokens for this user
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user token" });

            var ip = GetClientIp();
            await _authService.RevokeAllRefreshTokensForUserAsync(userId, ip);

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

            // Audit log
            await _auditService.LogAsync(
                userId,
                username,
                role,
                "Logout",
                "Authentication",
                null,
                null,
                $"User logged out from IP: {ip}",
                ip,
                Request.Headers["User-Agent"].FirstOrDefault(),
                AuditLogType.Authentication
            );

            return Ok(new { message = "Logout successful" });
        }

        // Debug -> list users in DB
        [HttpGet("debug/users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    PasswordHashLength = u.PasswordHash.Length,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                message = "Debug endpoint - users in database",
                userCount = users.Count,
                users
            });
        }

        private string? GetClientIp()
        {
            var fwd = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(fwd))
                return fwd.Split(',')[0].Trim();

            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            return !string.IsNullOrEmpty(realIp)
                ? realIp
                : Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
