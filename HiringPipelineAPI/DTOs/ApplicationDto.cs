namespace HiringPipelineAPI.DTOs;

// API-specific DTOs for Application operations
// These are only used by controllers and not shared with Core

/// <summary>
/// DTO for creating a new application
/// </summary>
public class CreateApplicationDto
{
    /// <summary>
    /// The ID of the candidate applying
    /// </summary>
    public int CandidateId { get; set; }
    
    /// <summary>
    /// The ID of the requisition being applied to
    /// </summary>
    public int RequisitionId { get; set; }
    
    /// <summary>
    /// The current stage of the application
    /// </summary>
    public string? CurrentStage { get; set; }
    
    /// <summary>
    /// The status of the application
    /// </summary>
    public string? Status { get; set; }
}

/// <summary>
/// DTO for updating an existing application
/// </summary>
public class UpdateApplicationDto
{
    /// <summary>
    /// The current stage of the application
    /// </summary>
    public string? CurrentStage { get; set; }
    
    /// <summary>
    /// The status of the application
    /// </summary>
    public string? Status { get; set; }
}
