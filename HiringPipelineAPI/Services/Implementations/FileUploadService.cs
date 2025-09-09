using Microsoft.AspNetCore.Http;
using HiringPipelineAPI.Services.Interfaces;

namespace HiringPipelineAPI.Services.Implementations;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadService> _logger;
    private const string UploadsFolder = "uploads";
    private const string ResumesFolder = "resumes";

    public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<FileUploadResult> UploadResumeAsync(IFormFile file)
    {
        try
        {
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsFolder, ResumesFolder);
            Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Save file
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Return relative path for database storage
            var relativePath = Path.Combine(UploadsFolder, ResumesFolder, uniqueFileName).Replace("\\", "/");

            _logger.LogInformation("Resume uploaded successfully: {FileName} -> {FilePath}", file.FileName, relativePath);

            return new FileUploadResult
            {
                FileName = file.FileName,
                FilePath = relativePath,
                FileSize = file.Length,
                ContentType = file.ContentType
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading resume file: {FileName}", file.FileName);
            throw;
        }
    }

    public async Task<byte[]> DownloadResumeAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
            
            if (!System.IO.File.Exists(fullPath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return await System.IO.File.ReadAllBytesAsync(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading resume file: {FilePath}", filePath);
            throw;
        }
    }

    public Task<bool> DeleteResumeAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
            
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                _logger.LogInformation("Resume file deleted: {FilePath}", filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting resume file: {FilePath}", filePath);
            return Task.FromResult(false);
        }
    }
}
