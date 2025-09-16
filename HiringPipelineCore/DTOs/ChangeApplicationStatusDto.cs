using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.DTOs
{
    /// <summary>
    /// DTO for changing application status
    /// </summary>
    public class ChangeApplicationStatusDto
    {
        /// <summary>
        /// The new status for the application
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Optional notes about the status change
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
