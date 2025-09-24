using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.DTOs;

// Core business DTOs for Application entity

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

public class ApplicationDto
{
    public int ApplicationId { get; set; }
    public int CandidateId { get; set; }
    public int RequisitionId { get; set; }
    public string CurrentStage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ApplicationDetailDto : ApplicationDto
{
    public CandidateSummaryDto Candidate { get; set; } = new();
    public RequisitionSummaryDto Requisition { get; set; } = new();
    public List<StageHistorySummaryDto> StageHistory { get; set; } = new();
}

// Summary DTOs for use across the domain
public class ApplicationSummaryDto
{
    public int ApplicationId { get; set; }
    public string CurrentStage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
