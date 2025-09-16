using Microsoft.EntityFrameworkCore;
using HiringPipelineCore.Entities;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineInfrastructure.Data;
using System.Text.Json;

namespace HiringPipelineInfrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly HiringPipelineDbContext _context;

        public AuditService(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            int userId,
            string username,
            string userRole,
            string action,
            string entity,
            int? entityId = null,
            string? changes = null,
            string? details = null,
            string? ipAddress = null,
            string? userAgent = null,
            AuditLogType logType = AuditLogType.UserAction)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Username = username,
                UserRole = userRole,
                Action = action,
                Entity = entity,
                EntityId = entityId,
                Changes = changes,
                Details = details,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LogType = logType,
                Timestamp = DateTime.Now
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogSimpleAsync(int userId, string username, string userRole, string action, string entity, int? entityId = null, string? details = null)
        {
            await LogAsync(userId, username, userRole, action, entity, entityId, null, details);
        }

        public async Task LogEntityChangesAsync<T>(
            int userId,
            string username,
            string userRole,
            string action,
            string entity,
            int entityId,
            T? beforeEntity,
            T? afterEntity,
            string? details = null,
            string? ipAddress = null,
            string? userAgent = null) where T : class
        {
            var changes = new Dictionary<string, object>();

            if (beforeEntity != null && afterEntity != null)
            {
                var beforeProps = typeof(T).GetProperties();
                var afterProps = typeof(T).GetProperties();

                foreach (var prop in beforeProps)
                {
                    var beforeValue = prop.GetValue(beforeEntity);
                    var afterValue = afterProps.First(p => p.Name == prop.Name).GetValue(afterEntity);

                    if (!Equals(beforeValue, afterValue))
                    {
                        changes[prop.Name] = new
                        {
                            Before = beforeValue,
                            After = afterValue
                        };
                    }
                }
            }
            else if (afterEntity != null)
            {
                // Create operation - log all properties
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    var value = prop.GetValue(afterEntity);
                    changes[prop.Name] = new { After = value };
                }
            }
            else if (beforeEntity != null)
            {
                // Delete operation - log all properties
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    var value = prop.GetValue(beforeEntity);
                    changes[prop.Name] = new { Before = value };
                }
            }

            var changesJson = changes.Any() ? JsonSerializer.Serialize(changes, new JsonSerializerOptions { WriteIndented = true }) : null;

            await LogAsync(userId, username, userRole, action, entity, entityId, changesJson, details, ipAddress, userAgent);
        }

        public async Task<List<AuditLog>> GetAuditLogsAsync(
            int? userId = null,
            string? entity = null,
            int? entityId = null,
            string? action = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            AuditLogType? logType = null,
            int skip = 0,
            int take = 100)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (!string.IsNullOrEmpty(entity))
                query = query.Where(a => a.Entity == entity);

            if (entityId.HasValue)
                query = query.Where(a => a.EntityId == entityId.Value);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);

            if (fromDate.HasValue)
                query = query.Where(a => a.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.Timestamp <= toDate.Value);

            if (logType.HasValue)
                query = query.Where(a => a.LogType == logType.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetAuditLogsCountAsync(
            int? userId = null,
            string? entity = null,
            int? entityId = null,
            string? action = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            AuditLogType? logType = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (!string.IsNullOrEmpty(entity))
                query = query.Where(a => a.Entity == entity);

            if (entityId.HasValue)
                query = query.Where(a => a.EntityId == entityId.Value);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);

            if (fromDate.HasValue)
                query = query.Where(a => a.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.Timestamp <= toDate.Value);

            if (logType.HasValue)
                query = query.Where(a => a.LogType == logType.Value);

            return await query.CountAsync();
        }

        public async Task ClearAllAuditLogsAsync()
        {
            var allLogs = await _context.AuditLogs.ToListAsync();
            _context.AuditLogs.RemoveRange(allLogs);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAuditLogAsync(int id)
        {
            var auditLog = await _context.AuditLogs.FindAsync(id);
            if (auditLog == null)
                return false;

            _context.AuditLogs.Remove(auditLog);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
