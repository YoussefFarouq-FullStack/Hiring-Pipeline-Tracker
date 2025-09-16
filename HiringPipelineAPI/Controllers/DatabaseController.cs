using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineInfrastructure.Data;

namespace HiringPipelineAPI.Controllers;

/// <summary>
/// Request model for clearing selected tables
/// </summary>
public class ClearTablesRequest
{
    /// <summary>
    /// List of table names to clear
    /// </summary>
    public List<string> TableNames { get; set; } = new();
}

/// <summary>
/// Request model for customizable database seeding
/// </summary>
public class SeedDataRequest
{
    /// <summary>
    /// Number of candidates to create
    /// </summary>
    public int CandidatesCount { get; set; } = 0;
    
    /// <summary>
    /// Number of requisitions to create
    /// </summary>
    public int RequisitionsCount { get; set; } = 0;
    
    /// <summary>
    /// Number of applications to create
    /// </summary>
    public int ApplicationsCount { get; set; } = 0;
    
    /// <summary>
    /// Number of stage history entries to create
    /// </summary>
    public int StageHistoryCount { get; set; } = 0;
    
    /// <summary>
    /// Whether to create additional users (beyond existing ones)
    /// </summary>
    public bool CreateUsers { get; set; } = false;
}

/// <summary>
/// Manages database maintenance operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // Require authentication for all database operations
public class DatabaseController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<DatabaseController> _logger;
    private readonly HiringPipelineDbContext _context;

    public DatabaseController(IDatabaseService databaseService, ILogger<DatabaseController> logger, HiringPipelineDbContext context)
    {
        _databaseService = databaseService;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Gets the count of records in each table
    /// </summary>
    /// <returns>Dictionary with table names and their record counts</returns>
    /// <response code="200">Returns the table counts</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("counts")]
    [ProducesResponseType(typeof(Dictionary<string, int>), 200)]
    public async Task<ActionResult<Dictionary<string, int>>> GetTableCounts()
    {
        try
        {
            var counts = await _databaseService.GetTableCountsAsync();
            return Ok(counts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting table counts");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Clears selected tables from the database
    /// </summary>
    /// <param name="request">Request containing list of table names to clear</param>
    /// <returns>No content on successful clear</returns>
    /// <response code="204">If the selected data was successfully cleared</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("clear-selected")]
    [ProducesResponseType(204)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ClearSelectedTables([FromBody] ClearTablesRequest request)
    {
        try
        {
            _logger.LogInformation("Selective database clear operation initiated by user: {User} for tables: {Tables}", 
                User.Identity?.Name, string.Join(", ", request.TableNames));
            
            await _databaseService.ClearSelectedTablesAsync(request.TableNames);
            
            _logger.LogInformation("Selective database clear operation completed successfully by user: {User}", User.Identity?.Name);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing selected tables");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <returns>Success message with backup file information</returns>
    /// <response code="200">If the backup was successfully created</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("backup")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateBackup()
    {
        try
        {
            _logger.LogInformation("Database backup operation initiated by user: {User}", User.Identity?.Name);
            
            var backupInfo = await _databaseService.CreateBackupAsync();
            
            _logger.LogInformation("Database backup operation completed successfully by user: {User}", User.Identity?.Name);
            return Ok(new { message = "Database backup created successfully", fileName = backupInfo.FileName, fileSize = backupInfo.FileSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database backup");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Restores the database from a backup file
    /// </summary>
    /// <param name="file">The backup file to restore from</param>
    /// <returns>Success message</returns>
    /// <response code="200">If the database was successfully restored</response>
    /// <response code="400">If the backup file is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("restore")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> RestoreFromBackup(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No backup file provided" });
        }

        try
        {
            _logger.LogInformation("Database restore operation initiated by user: {User}", User.Identity?.Name);
            
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileContent = memoryStream.ToArray();
            
            await _databaseService.RestoreFromBackupAsync(file.FileName, fileContent);
            
            _logger.LogInformation("Database restore operation completed successfully by user: {User}", User.Identity?.Name);
            return Ok(new { message = "Database restored successfully from backup" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring database from backup");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Clears all hiring pipeline data while preserving user accounts and system data
    /// </summary>
    /// <returns>Success message</returns>
    /// <response code="200">If the hiring pipeline data was successfully cleared</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("clear-hiring-data")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ClearHiringPipelineData()
    {
        try
        {
            _logger.LogInformation("Hiring pipeline data clear operation initiated by user: {User}", User.Identity?.Name);
            
            await _databaseService.ClearHiringPipelineDataAsync();
            
            _logger.LogInformation("Hiring pipeline data clear operation completed successfully by user: {User}", User.Identity?.Name);
            return Ok(new { message = "Hiring pipeline data cleared successfully. User accounts and system data preserved." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing hiring pipeline data");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Clears only business data (candidates, requisitions, applications, stage history) while preserving all system data
    /// </summary>
    /// <returns>Success message</returns>
    /// <response code="200">If the business data was successfully cleared</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("clear-business-data")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ClearBusinessDataOnly()
    {
        try
        {
            _logger.LogInformation("Business data clear operation initiated by user: {User}", User.Identity?.Name);
            
            await _databaseService.ClearBusinessDataOnlyAsync();
            
            _logger.LogInformation("Business data clear operation completed successfully by user: {User}", User.Identity?.Name);
            return Ok(new { message = "Business data cleared successfully. All system data (users, roles, permissions, audit logs) preserved." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing business data");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Clears all data including users and roles (DANGEROUS - use with caution)
    /// </summary>
    /// <returns>Success message</returns>
    /// <response code="200">If all data was successfully cleared</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("clear-all-data")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ClearAllDataIncludingUsers()
    {
        try
        {
            _logger.LogWarning("DANGEROUS: Complete data clear operation initiated by user: {User}", User.Identity?.Name);
            
            await _databaseService.ClearAllDataIncludingUsersAsync();
            
            _logger.LogWarning("DANGEROUS: Complete data clear operation completed successfully by user: {User}", User.Identity?.Name);
            return Ok(new { message = "All data cleared successfully. WARNING: This includes all users, roles, and permissions!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all data");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Exports data from specific tables to CSV format
    /// </summary>
    /// <param name="request">Request containing list of table names to export</param>
    /// <returns>ZIP file containing CSV exports</returns>
    /// <response code="200">If the data was successfully exported</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("export")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ExportData([FromBody] ClearTablesRequest request)
    {
        if (request.TableNames == null || !request.TableNames.Any())
        {
            return BadRequest(new { message = "No tables specified for export" });
        }

        try
        {
            _logger.LogInformation("Data export operation initiated by user: {User} for tables: {Tables}", 
                User.Identity?.Name, string.Join(", ", request.TableNames));
            
            var exportData = await _databaseService.ExportTablesAsync(request.TableNames);
            var fileName = $"data_export_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            
            _logger.LogInformation("Data export operation completed successfully by user: {User}", User.Identity?.Name);
            return File(exportData, "application/zip", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Seeds the database with initial test data (fixed amounts)
    /// </summary>
    /// <returns>Success message with seeding details</returns>
    /// <response code="200">Database seeded successfully</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("seed")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult> SeedDatabase()
    {
        try
        {
            _logger.LogInformation("Database seeding operation initiated by user: {User}", User.Identity?.Name);
            
            await DbInitializer.SeedAsync(_context, isDevelopment: true);
            
            _logger.LogInformation("Database seeding operation completed successfully by user: {User}", User.Identity?.Name);
            
            return Ok(new { 
                success = true, 
                message = "Database seeded successfully with test data",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            return StatusCode(500, new { 
                success = false, 
                message = "Failed to seed database", 
                error = ex.Message 
            });
        }
    }

    /// <summary>
    /// Seeds the database with customizable amounts of test data
    /// </summary>
    /// <param name="request">Request specifying what data to create and how much</param>
    /// <returns>Success message with seeding details</returns>
    /// <response code="200">Database seeded successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("seed-custom")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> SeedDatabaseCustom([FromBody] SeedDataRequest request)
    {
        try
        {
            _logger.LogInformation("Custom database seeding operation initiated by user: {User}", User.Identity?.Name);
            
            // Validate request
            if (request.CandidatesCount < 0 || request.RequisitionsCount < 0 || 
                request.ApplicationsCount < 0 || request.StageHistoryCount < 0)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "Count values must be non-negative" 
                });
            }

            var result = await DbInitializer.SeedCustomAsync(_context, request);
            
            _logger.LogInformation("Custom database seeding operation completed successfully by user: {User}", User.Identity?.Name);
            
            return Ok(new { 
                success = true, 
                message = "Custom database seeding completed successfully",
                data = result,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in custom database seeding");
            return StatusCode(500, new { 
                success = false, 
                message = "Failed to seed database with custom data", 
                error = ex.Message 
            });
        }
    }
}
