namespace HiringPipelineCore.DTOs
{
    /// <summary>
    /// DTO for database backup information
    /// </summary>
    public class BackupInfoDto
    {
        /// <summary>
        /// The name of the backup file
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// The size of the backup file in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The path where the backup is stored
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the backup was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
