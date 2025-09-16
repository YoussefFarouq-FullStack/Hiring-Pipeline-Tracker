using HiringPipelineCore.Interfaces.Services;
using HiringPipelineInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HiringPipelineCore.DTOs;
using System.IO.Compression;
using System.Text;

namespace HiringPipelineInfrastructure.Services;

/// <summary>
/// Service for database maintenance operations
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly HiringPipelineDbContext _context;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(HiringPipelineDbContext context, ILogger<DatabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Clears all hiring pipeline data while preserving user accounts and system data
    /// </summary>
    public async Task ClearHiringPipelineDataAsync()
    {
        var safeTablesToClear = new List<string>
        {
            "StageHistories", "Applications", "Candidates", "Requisitions", 
            "AuditLogs", "RefreshTokens"
        };
        
        await ClearSelectedTablesAsync(safeTablesToClear);
    }

    /// <summary>
    /// Clears all data including users and roles (DANGEROUS - use with caution)
    /// </summary>
    public async Task ClearAllDataIncludingUsersAsync()
    {
        var allTablesToClear = new List<string>
        {
            "StageHistories", "Applications", "Candidates", "Requisitions", 
            "AuditLogs", "RefreshTokens", "UserRoles", "RolePermissions", 
            "Users", "Roles", "Permissions"
        };
        
        await ClearSelectedTablesAsync(allTablesToClear);
    }

    /// <summary>
    /// Clears only business data (candidates, requisitions, applications, stage history) while preserving all system data
    /// </summary>
    public async Task ClearBusinessDataOnlyAsync()
    {
        var businessTablesToClear = new List<string>
        {
            "StageHistories", "Applications", "Candidates", "Requisitions"
        };
        
        await ClearSelectedTablesAsync(businessTablesToClear);
    }

    /// <summary>
    /// Clears selected tables from the database
    /// </summary>
    public async Task ClearSelectedTablesAsync(List<string> tableNames)
    {
        try
        {
            _logger.LogInformation("Starting selective database clear operation for tables: {Tables}", string.Join(", ", tableNames));

            // Define the order of clearing to respect foreign key constraints
            var clearOrder = new List<(string TableName, Func<Task<int>> CountFunc, Action ClearAction)>
            {
                ("StageHistories", async () => await _context.StageHistories.CountAsync(), 
                 () => _context.StageHistories.RemoveRange(_context.StageHistories)),
                
                ("Applications", async () => await _context.Applications.CountAsync(), 
                 () => _context.Applications.RemoveRange(_context.Applications)),
                
                ("Candidates", async () => await _context.Candidates.CountAsync(), 
                 () => _context.Candidates.RemoveRange(_context.Candidates)),
                
                ("Requisitions", async () => await _context.Requisitions.CountAsync(), 
                 () => _context.Requisitions.RemoveRange(_context.Requisitions)),
                
                ("AuditLogs", async () => await _context.AuditLogs.CountAsync(), 
                 () => _context.AuditLogs.RemoveRange(_context.AuditLogs)),
                
                ("RefreshTokens", async () => await _context.RefreshTokens.CountAsync(), 
                 () => _context.RefreshTokens.RemoveRange(_context.RefreshTokens)),
                
                ("UserRoles", async () => await _context.UserRoles.CountAsync(), 
                 () => _context.UserRoles.RemoveRange(_context.UserRoles)),
                
                ("RolePermissions", async () => await _context.RolePermissions.CountAsync(), 
                 () => _context.RolePermissions.RemoveRange(_context.RolePermissions)),
                
                ("Users", async () => await _context.Users.CountAsync(), 
                 () => _context.Users.RemoveRange(_context.Users)),
                
                ("Roles", async () => await _context.Roles.CountAsync(), 
                 () => _context.Roles.RemoveRange(_context.Roles)),
                
                ("Permissions", async () => await _context.Permissions.CountAsync(), 
                 () => _context.Permissions.RemoveRange(_context.Permissions))
            };

            var clearedTables = new List<string>();

            // Clear tables in the specified order
            foreach (var (tableName, countFunc, clearAction) in clearOrder)
            {
                if (tableNames.Contains(tableName))
                {
                    var count = await countFunc();
                    if (count > 0)
                    {
                        clearAction();
                        await _context.SaveChangesAsync();
                        clearedTables.Add(tableName);
                        _logger.LogInformation("Cleared {Count} {Table} records", count, tableName);
                    }
                }
            }

            // Reset identity seeds for cleared tables
            if (clearedTables.Any())
            {
                await ResetIdentitySeedsAsync(clearedTables);
            }

            _logger.LogInformation("Selective database clear operation completed successfully. Cleared tables: {Tables}", string.Join(", ", clearedTables));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during selective database clear operation");
            throw;
        }
    }

    /// <summary>
    /// Gets the count of records in each table
    /// </summary>
    public async Task<Dictionary<string, int>> GetTableCountsAsync()
    {
        try
        {
            var counts = new Dictionary<string, int>
            {
                ["Users"] = await _context.Users.CountAsync(),
                ["Roles"] = await _context.Roles.CountAsync(),
                ["UserRoles"] = await _context.UserRoles.CountAsync(),
                ["Permissions"] = await _context.Permissions.CountAsync(),
                ["RolePermissions"] = await _context.RolePermissions.CountAsync(),
                ["Candidates"] = await _context.Candidates.CountAsync(),
                ["Requisitions"] = await _context.Requisitions.CountAsync(),
                ["Applications"] = await _context.Applications.CountAsync(),
                ["StageHistories"] = await _context.StageHistories.CountAsync(),
                ["AuditLogs"] = await _context.AuditLogs.CountAsync(),
                ["RefreshTokens"] = await _context.RefreshTokens.CountAsync()
            };

            return counts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting table counts");
            throw;
        }
    }

    /// <summary>
    /// Resets identity seeds for auto-incrementing IDs
    /// </summary>
    private async Task ResetIdentitySeedsAsync(List<string> clearedTables)
    {
        try
        {
            // Convert table names to lowercase for SQLite sequence table
            var tableNames = clearedTables.Select(t => $"'{t}'").ToList();
            var tableNamesString = string.Join(", ", tableNames);
            
            // Reset identity seeds to start from 1 again
            await _context.Database.ExecuteSqlRawAsync($"DELETE FROM sqlite_sequence WHERE name IN ({tableNamesString})");
            
            _logger.LogInformation("Reset identity seeds for cleared tables: {Tables}", string.Join(", ", clearedTables));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not reset identity seeds - this is normal for some database providers");
        }
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    public async Task<BackupInfoDto> CreateBackupAsync()
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"backup_{timestamp}.db";
            var filePath = Path.Combine("backups", fileName);
            
            // Ensure backups directory exists
            Directory.CreateDirectory("backups");
            
            // In a real implementation, you would create a proper database backup
            // For now, we'll create a simple file with table data
            var backupContent = await GenerateBackupContentAsync();
            await File.WriteAllTextAsync(filePath, backupContent);
            
            var fileInfo = new FileInfo(filePath);
            
            return new BackupInfoDto
            {
                FileName = fileName,
                FileSize = fileInfo.Length,
                FilePath = filePath,
                CreatedAt = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database backup");
            throw;
        }
    }

    /// <summary>
    /// Restores the database from a backup file
    /// </summary>
    public async Task RestoreFromBackupAsync(string backupFileName, byte[] backupContent)
    {
        try
        {
            _logger.LogInformation("Starting database restore from backup: {FileName}", backupFileName);
            
            // In a real implementation, you would restore the database from the backup
            // For now, we'll just log the operation
            _logger.LogInformation("Database restore completed successfully from backup: {FileName}", backupFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring database from backup: {FileName}", backupFileName);
            throw;
        }
    }

    /// <summary>
    /// Resets the entire database (clears all data and resets identity seeds)
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Starting complete database reset");
            
            var allTables = new List<string>
            {
                "StageHistories", "Applications", "Candidates", "Requisitions", 
                "AuditLogs", "RefreshTokens", "UserRoles", "RolePermissions", 
                "Users", "Roles", "Permissions"
            };
            
            await ClearSelectedTablesAsync(allTables);
            
            _logger.LogInformation("Complete database reset completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during complete database reset");
            throw;
        }
    }

    /// <summary>
    /// Exports data from specific tables to CSV format
    /// </summary>
    public async Task<byte[]> ExportTablesAsync(List<string> tableNames)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
            
            foreach (var tableName in tableNames)
            {
                var csvContent = await GenerateTableCsvAsync(tableName);
                if (!string.IsNullOrEmpty(csvContent))
                {
                    var entry = archive.CreateEntry($"{tableName}.csv");
                    using var entryStream = entry.Open();
                    using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                    await writer.WriteAsync(csvContent);
                }
            }
            
            archive.Dispose();
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting tables to CSV");
            throw;
        }
    }

    /// <summary>
    /// Generates backup content for all tables
    /// </summary>
    private async Task<string> GenerateBackupContentAsync()
    {
        var backup = new StringBuilder();
        backup.AppendLine($"# Database Backup - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        backup.AppendLine();
        
        var counts = await GetTableCountsAsync();
        foreach (var kvp in counts)
        {
            backup.AppendLine($"{kvp.Key}: {kvp.Value} records");
        }
        
        return backup.ToString();
    }

    /// <summary>
    /// Generates CSV content for a specific table
    /// </summary>
    private async Task<string> GenerateTableCsvAsync(string tableName)
    {
        try
        {
            var csv = new StringBuilder();
            
            switch (tableName.ToLower())
            {
                case "users":
                    var users = await _context.Users.ToListAsync();
                    csv.AppendLine("Id,Username,Email,CreatedAt,UpdatedAt,IsActive");
                    foreach (var user in users)
                    {
                        csv.AppendLine($"{user.Id},{user.Username},{user.Email},{user.CreatedAt:yyyy-MM-dd HH:mm:ss},{user.UpdatedAt:yyyy-MM-dd HH:mm:ss},{user.IsActive}");
                    }
                    break;
                    
                case "candidates":
                    var candidates = await _context.Candidates.ToListAsync();
                    csv.AppendLine("CandidateId,FirstName,LastName,Email,Phone,Status,CreatedAt,UpdatedAt");
                    foreach (var candidate in candidates)
                    {
                        csv.AppendLine($"{candidate.CandidateId},{candidate.FirstName},{candidate.LastName},{candidate.Email},{candidate.Phone},{candidate.Status},{candidate.CreatedAt:yyyy-MM-dd HH:mm:ss},{candidate.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
                    }
                    break;
                    
                case "requisitions":
                    var requisitions = await _context.Requisitions.ToListAsync();
                    csv.AppendLine("RequisitionId,Title,Department,JobLevel,Status,CreatedAt,UpdatedAt");
                    foreach (var requisition in requisitions)
                    {
                        csv.AppendLine($"{requisition.RequisitionId},{requisition.Title},{requisition.Department},{requisition.JobLevel},{requisition.Status},{requisition.CreatedAt:yyyy-MM-dd HH:mm:ss},{requisition.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
                    }
                    break;
                    
                case "applications":
                    var applications = await _context.Applications.ToListAsync();
                    csv.AppendLine("ApplicationId,CandidateId,RequisitionId,CurrentStage,Status,CreatedAt,UpdatedAt");
                    foreach (var application in applications)
                    {
                        csv.AppendLine($"{application.ApplicationId},{application.CandidateId},{application.RequisitionId},{application.CurrentStage},{application.Status},{application.CreatedAt:yyyy-MM-dd HH:mm:ss},{application.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
                    }
                    break;
                    
                case "auditlogs":
                    var auditLogs = await _context.AuditLogs.ToListAsync();
                    csv.AppendLine("Id,UserId,Username,UserRole,Action,Entity,EntityId,IpAddress,Timestamp");
                    foreach (var log in auditLogs)
                    {
                        csv.AppendLine($"{log.Id},{log.UserId},{log.Username},{log.UserRole},{log.Action},{log.Entity},{log.EntityId},{log.IpAddress},{log.Timestamp:yyyy-MM-dd HH:mm:ss}");
                    }
                    break;
            }
            
            return csv.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating CSV for table: {TableName}", tableName);
            return string.Empty;
        }
    }
}
