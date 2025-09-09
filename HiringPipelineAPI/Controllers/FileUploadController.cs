using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HiringPipelineAPI.Services.Interfaces;

namespace HiringPipelineAPI.Controllers;

/// <summary>
/// Handles file upload operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class FileUploadController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;

    /// <summary>
    /// Initializes a new instance of the FileUploadController
    /// </summary>
    /// <param name="fileUploadService">The file upload service</param>
    public FileUploadController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    /// <summary>
    /// Uploads a candidate resume file
    /// </summary>
    /// <param name="file">The resume file to upload</param>
    /// <returns>File upload result with file path and filename</returns>
    /// <response code="200">File uploaded successfully</response>
    /// <response code="400">Invalid file or file too large</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("resume")]
    [ProducesResponseType(typeof(FileUploadResult), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<FileUploadResult>> UploadResume(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Validate file type
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest("Invalid file type. Only PDF, DOC, and DOCX files are allowed.");
        }

        // Validate file size (5MB limit)
        const long maxFileSize = 5 * 1024 * 1024; // 5MB
        if (file.Length > maxFileSize)
        {
            return BadRequest("File size exceeds the 5MB limit.");
        }

        try
        {
            var result = await _fileUploadService.UploadResumeAsync(file);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while uploading the file: {ex.Message}");
        }
    }

    /// <summary>
    /// Downloads a candidate resume file
    /// </summary>
    /// <param name="filePath">The path to the file to download</param>
    /// <returns>The file content</returns>
    /// <response code="200">File downloaded successfully</response>
    /// <response code="404">File not found</response>
    [HttpGet("resume/{*filePath}")]
    [ProducesResponseType(typeof(File), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DownloadResume(string filePath)
    {
        try
        {
            var fileBytes = await _fileUploadService.DownloadResumeAsync(filePath);
            var fileName = Path.GetFileName(filePath);
            var contentType = GetContentType(filePath);
            
            return File(fileBytes, contentType, fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("File not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while downloading the file: {ex.Message}");
        }
    }

    private static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }
}

/// <summary>
/// Represents the result of a file upload operation
/// </summary>
public class FileUploadResult
{
    /// <summary>
    /// The original filename of the uploaded file
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// The server path where the file is stored
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// The size of the uploaded file in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// The MIME content type of the uploaded file
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
}
