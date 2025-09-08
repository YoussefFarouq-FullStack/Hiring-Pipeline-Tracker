namespace HiringPipelineAPI.DTOs;

// API-specific DTOs for Application operations
// These are only used by controllers and not shared with Core

// Create DTOs (what the frontend sends to create)
public class CreateApplicationDto
{
    public int CandidateId { get; set; }
    public int RequisitionId { get; set; }
    public string? CurrentStage { get; set; }
    public string? Status { get; set; }
}

// Update DTOs (what the frontend sends to update)
public class UpdateApplicationDto
{
    public string? CurrentStage { get; set; }
    public string? Status { get; set; }
}
