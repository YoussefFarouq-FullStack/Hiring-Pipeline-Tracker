using System.ComponentModel.DataAnnotations;
using HiringPipelineCore.Entities;

namespace HiringPipelineCore.DTOs
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string? Changes { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string? UserAgent { get; set; }
        public AuditLogType LogType { get; set; }
    }

    public class AuditLogFilterDto
    {
        public int? UserId { get; set; }
        public string? Entity { get; set; }
        public int? EntityId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 100;
    }

    public class AuditLogResponseDto
    {
        public List<AuditLogDto> AuditLogs { get; set; } = new List<AuditLogDto>();
        public int TotalCount { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
