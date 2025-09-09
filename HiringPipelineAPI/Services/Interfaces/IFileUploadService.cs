namespace HiringPipelineAPI.Services.Interfaces;

public interface IFileUploadService
{
    Task<FileUploadResult> UploadResumeAsync(IFormFile file);
    Task<byte[]> DownloadResumeAsync(string filePath);
    Task<bool> DeleteResumeAsync(string filePath);
}

public class FileUploadResult
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
}
