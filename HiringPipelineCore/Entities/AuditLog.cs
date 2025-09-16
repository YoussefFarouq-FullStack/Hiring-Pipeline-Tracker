using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiringPipelineCore.Entities
{
    /// <summary>
    /// Audit log entity for tracking all system changes
    /// </summary>
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID of the user who performed the action
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Username of the user who performed the action
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Role of the user who performed the action
        /// </summary>
        [MaxLength(50)]
        public string UserRole { get; set; } = string.Empty;

        /// <summary>
        /// IP address of the user
        /// </summary>
        [MaxLength(45)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// Type of action performed (Create, Update, Delete, Login, etc.)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Entity type that was affected (Candidate, Requisition, Application, etc.)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Entity { get; set; } = string.Empty;

        /// <summary>
        /// ID of the specific entity instance that was affected
        /// </summary>
        public int? EntityId { get; set; }

        /// <summary>
        /// JSON string containing the changes made (before/after values)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Changes { get; set; }

        /// <summary>
        /// Additional details about the action
        /// </summary>
        [MaxLength(500)]
        public string? Details { get; set; }

        /// <summary>
        /// Timestamp when the action occurred
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User agent string from the request
        /// </summary>
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Type of log entry (UserAction, SystemOperation, BackgroundFetch)
        /// </summary>
        [Required]
        public AuditLogType LogType { get; set; } = AuditLogType.UserAction;

        /// <summary>
        /// Navigation property to the user
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
