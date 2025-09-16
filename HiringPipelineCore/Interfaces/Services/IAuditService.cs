using HiringPipelineCore.Entities;

namespace HiringPipelineCore.Interfaces.Services
{
    /// <summary>
    /// Service for logging audit trails
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Log an audit entry
        /// </summary>
        /// <param name="userId">ID of the user performing the action</param>
        /// <param name="username">Username of the user</param>
        /// <param name="userRole">Role of the user</param>
        /// <param name="action">Action performed (Create, Update, Delete, etc.)</param>
        /// <param name="entity">Entity type affected</param>
        /// <param name="entityId">ID of the specific entity instance</param>
        /// <param name="changes">JSON string of changes made</param>
        /// <param name="details">Additional details</param>
        /// <param name="ipAddress">IP address of the user</param>
        /// <param name="userAgent">User agent string</param>
        /// <param name="logType">Type of log entry</param>
        Task LogAsync(
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
            AuditLogType logType = AuditLogType.UserAction);

        /// <summary>
        /// Log a simple action without detailed changes
        /// </summary>
        Task LogSimpleAsync(int userId, string username, string userRole, string action, string entity, int? entityId = null, string? details = null);

        /// <summary>
        /// Log entity changes with before/after comparison
        /// </summary>
        Task LogEntityChangesAsync<T>(
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
            string? userAgent = null) where T : class;

        /// <summary>
        /// Get audit logs with filtering options
        /// </summary>
        Task<List<AuditLog>> GetAuditLogsAsync(
            int? userId = null,
            string? entity = null,
            int? entityId = null,
            string? action = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            AuditLogType? logType = null,
            int skip = 0,
            int take = 100);

        /// <summary>
        /// Get audit logs count for pagination
        /// </summary>
        Task<int> GetAuditLogsCountAsync(
            int? userId = null,
            string? entity = null,
            int? entityId = null,
            string? action = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            AuditLogType? logType = null);

        /// <summary>
        /// Clear all audit logs
        /// </summary>
        Task ClearAllAuditLogsAsync();

        /// <summary>
        /// Delete a specific audit log entry
        /// </summary>
        /// <param name="id">ID of the audit log entry to delete</param>
        /// <returns>True if deleted successfully, false if not found</returns>
        Task<bool> DeleteAuditLogAsync(int id);
    }
}
