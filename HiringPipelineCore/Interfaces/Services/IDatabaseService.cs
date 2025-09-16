namespace HiringPipelineCore.Interfaces.Services;

using HiringPipelineCore.DTOs;

/// <summary>
/// Service for database maintenance operations
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Clears all hiring pipeline data while preserving user accounts and system data
    /// </summary>
    /// <returns>Task representing the operation</returns>
    Task ClearHiringPipelineDataAsync();

    /// <summary>
    /// Clears all data including users and roles (DANGEROUS - use with caution)
    /// </summary>
    /// <returns>Task representing the operation</returns>
    Task ClearAllDataIncludingUsersAsync();

    /// <summary>
    /// Clears only business data (candidates, requisitions, applications, stage history) while preserving all system data
    /// </summary>
    /// <returns>Task representing the operation</returns>
    Task ClearBusinessDataOnlyAsync();

    /// <summary>
    /// Clears selected tables from the database
    /// </summary>
    /// <param name="tableNames">List of table names to clear</param>
    /// <returns>Task representing the operation</returns>
    Task ClearSelectedTablesAsync(List<string> tableNames);

    /// <summary>
    /// Gets the count of records in each table
    /// </summary>
    /// <returns>Dictionary with table names and their record counts</returns>
    Task<Dictionary<string, int>> GetTableCountsAsync();

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <returns>Backup information including file details</returns>
    Task<BackupInfoDto> CreateBackupAsync();

    /// <summary>
    /// Restores the database from a backup file
    /// </summary>
    /// <param name="backupFileName">The name of the backup file</param>
    /// <param name="backupContent">The content of the backup file</param>
    /// <returns>Task representing the operation</returns>
    Task RestoreFromBackupAsync(string backupFileName, byte[] backupContent);

    /// <summary>
    /// Resets the entire database (clears all data and resets identity seeds)
    /// </summary>
    /// <returns>Task representing the operation</returns>
    Task ResetDatabaseAsync();

    /// <summary>
    /// Exports data from specific tables to CSV format
    /// </summary>
    /// <param name="tableNames">List of table names to export</param>
    /// <returns>ZIP file containing CSV exports</returns>
    Task<byte[]> ExportTablesAsync(List<string> tableNames);
}
