using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.DTOs
{
    /// <summary>
    /// DTO for moving candidate to a different stage
    /// </summary>
    public class MoveToStageDto
    {
        /// <summary>
        /// The stage to move the candidate to
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ToStage { get; set; } = string.Empty;

        /// <summary>
        /// Optional notes about the stage movement
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// The user who initiated the movement
        /// </summary>
        [Required]
        public int MovedByUserId { get; set; }
    }
}
