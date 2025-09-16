namespace HiringPipelineCore.DTOs
{
    /// <summary>
    /// DTO for logging user actions from the frontend
    /// </summary>
    public class UserActionLogDto
    {
        /// <summary>
        /// The action performed by the user
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// The entity affected by the action
        /// </summary>
        public string Entity { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the specific entity (optional)
        /// </summary>
        public int? EntityId { get; set; }

        /// <summary>
        /// Additional details about the action (optional)
        /// </summary>
        public string? Details { get; set; }
    }
}
