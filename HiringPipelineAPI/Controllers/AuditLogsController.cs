using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Entities;

namespace HiringPipelineAPI.Controllers
{
    /// <summary>
    /// Audit logs management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")] // Only admins can view audit logs
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditLogsController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        /// <summary>
        /// Get audit logs with filtering options
        /// </summary>
        /// <param name="userId">Filter by user ID</param>
        /// <param name="entity">Filter by entity type</param>
        /// <param name="entityId">Filter by specific entity ID</param>
        /// <param name="action">Filter by action type</param>
        /// <param name="fromDate">Filter from date</param>
        /// <param name="toDate">Filter to date</param>
        /// <param name="logType">Filter by log type</param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take</param>
        /// <returns>Paginated audit logs</returns>
        [HttpGet]
        [ProducesResponseType(typeof(AuditLogResponseDto), 200)]
        public async Task<ActionResult<AuditLogResponseDto>> GetAuditLogs(
            [FromQuery] int? userId = null,
            [FromQuery] string? entity = null,
            [FromQuery] int? entityId = null,
            [FromQuery] string? action = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] AuditLogType? logType = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 100)
        {
            var auditLogs = await _auditService.GetAuditLogsAsync(
                userId, entity, entityId, action, fromDate, toDate, logType, skip, take);

            var totalCount = await _auditService.GetAuditLogsCountAsync(
                userId, entity, entityId, action, fromDate, toDate, logType);

            var auditLogDtos = auditLogs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Username = log.Username,
                UserRole = log.UserRole,
                IpAddress = log.IpAddress,
                Action = log.Action,
                Entity = log.Entity,
                EntityId = log.EntityId,
                Changes = log.Changes,
                Details = log.Details,
                Timestamp = log.Timestamp,
                UserAgent = log.UserAgent,
                LogType = log.LogType
            }).ToList();

            var response = new AuditLogResponseDto
            {
                AuditLogs = auditLogDtos,
                TotalCount = totalCount,
                Skip = skip,
                Take = take
            };

            return Ok(response);
        }

        /// <summary>
        /// Get audit logs for a specific entity
        /// </summary>
        /// <param name="entity">Entity type</param>
        /// <param name="entityId">Entity ID</param>
        /// <returns>Audit logs for the specific entity</returns>
        [HttpGet("entity/{entity}/{entityId}")]
        [ProducesResponseType(typeof(List<AuditLogDto>), 200)]
        public async Task<ActionResult<List<AuditLogDto>>> GetEntityAuditLogs(string entity, int entityId)
        {
            var auditLogs = await _auditService.GetAuditLogsAsync(
                entity: entity, entityId: entityId, take: 50);

            var auditLogDtos = auditLogs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Username = log.Username,
                UserRole = log.UserRole,
                IpAddress = log.IpAddress,
                Action = log.Action,
                Entity = log.Entity,
                EntityId = log.EntityId,
                Changes = log.Changes,
                Details = log.Details,
                Timestamp = log.Timestamp,
                UserAgent = log.UserAgent,
                LogType = log.LogType
            }).ToList();

            return Ok(auditLogDtos);
        }

        /// <summary>
        /// Get audit logs for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take</param>
        /// <returns>Audit logs for the specific user</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(AuditLogResponseDto), 200)]
        public async Task<ActionResult<AuditLogResponseDto>> GetUserAuditLogs(int userId, [FromQuery] int skip = 0, [FromQuery] int take = 100)
        {
            var auditLogs = await _auditService.GetAuditLogsAsync(
                userId: userId, skip: skip, take: take);

            var totalCount = await _auditService.GetAuditLogsCountAsync(userId: userId);

            var auditLogDtos = auditLogs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Username = log.Username,
                UserRole = log.UserRole,
                IpAddress = log.IpAddress,
                Action = log.Action,
                Entity = log.Entity,
                EntityId = log.EntityId,
                Changes = log.Changes,
                Details = log.Details,
                Timestamp = log.Timestamp,
                UserAgent = log.UserAgent,
                LogType = log.LogType
            }).ToList();

            var response = new AuditLogResponseDto
            {
                AuditLogs = auditLogDtos,
                TotalCount = totalCount,
                Skip = skip,
                Take = take
            };

            return Ok(response);
        }

        /// <summary>
        /// Clear all audit logs (Admin only)
        /// </summary>
        /// <returns>Success status</returns>
        /// <response code="200">Audit logs cleared successfully</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("clear")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<string>> ClearAllAuditLogs()
        {
            try
            {
                // Get count before clearing
                var count = await _auditService.GetAuditLogsCountAsync();
                
                // Clear all audit logs
                await _auditService.ClearAllAuditLogsAsync();
                
                return Ok($"Successfully cleared {count} audit log entries");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to clear audit logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a specific audit log entry (Admin only)
        /// </summary>
        /// <param name="id">Audit log entry ID</param>
        /// <returns>Success status</returns>
        /// <response code="200">Audit log entry deleted successfully</response>
        /// <response code="404">If the audit log entry was not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<string>> DeleteAuditLogEntry(int id)
        {
            try
            {
                var success = await _auditService.DeleteAuditLogAsync(id);
                if (!success)
                {
                    return NotFound("Audit log entry not found");
                }
                
                return Ok("Audit log entry deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to delete audit log entry: {ex.Message}");
            }
        }

        /// <summary>
        /// Export audit logs to CSV format (Admin only)
        /// </summary>
        /// <param name="userId">Filter by user ID</param>
        /// <param name="entity">Filter by entity type</param>
        /// <param name="action">Filter by action type</param>
        /// <param name="fromDate">Filter from date</param>
        /// <param name="toDate">Filter to date</param>
        /// <returns>CSV file download</returns>
        /// <response code="200">CSV file generated successfully</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("export")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportAuditLogs(
            [FromQuery] int? userId = null,
            [FromQuery] string? entity = null,
            [FromQuery] string? action = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsAsync(
                    userId, entity, null, action, fromDate, toDate, 0, int.MaxValue);

                var csvContent = GenerateCsvContent(auditLogs);
                var fileName = $"audit_logs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to export audit logs: {ex.Message}");
            }
        }

        private string GenerateCsvContent(List<AuditLog> auditLogs)
        {
            var csv = new System.Text.StringBuilder();
            
            // CSV Header
            csv.AppendLine("Id,Timestamp,Username,UserRole,Action,Entity,EntityId,IpAddress,UserAgent,Details,Changes");
            
            // CSV Data
            foreach (var log in auditLogs)
            {
                csv.AppendLine($"{log.Id},{log.Timestamp:yyyy-MM-dd HH:mm:ss},\"{log.Username}\",\"{log.UserRole}\",\"{log.Action}\",\"{log.Entity}\",{log.EntityId},\"{log.IpAddress}\",\"{log.UserAgent}\",\"{log.Details}\",\"{log.Changes}\"");
            }
            
            return csv.ToString();
        }

        /// <summary>
        /// Log a user action explicitly (for frontend-initiated actions)
        /// </summary>
        /// <param name="logData">The log data</param>
        /// <returns>Success response</returns>
        [HttpPost("log")]
        [ProducesResponseType(200)]
        [AllowAnonymous] // Allow frontend to log actions
        public async Task<ActionResult> LogUserAction([FromBody] UserActionLogDto logData)
        {
            try
            {
                await _auditService.LogAsync(
                    userId: User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value != null 
                        ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!) 
                        : 0,
                    username: User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Anonymous",
                    userRole: User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Unknown",
                    action: logData.Action,
                    entity: logData.Entity,
                    entityId: logData.EntityId,
                    changes: logData.Details,
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    userAgent: HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown",
                    logType: AuditLogType.UserAction
                );

                return Ok(new { success = true, message = "Action logged successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to log action", error = ex.Message });
            }
        }
    }
}
