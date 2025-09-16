namespace HiringPipelineCore.DTOs
{
    /// <summary>
    /// DTO for file upload result
    /// </summary>
    public class FileUploadResultDto
    {
        /// <summary>
        /// The name of the uploaded file
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// The size of the uploaded file in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The path where the file is stored
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
    }
}
