namespace HiringPipelineAPI.DTOs;

// API-specific DTOs for Requisition operations
// These are only used by controllers and not shared with Core

// Create DTOs (what the frontend sends to create)
public class CreateRequisitionDto
{
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? JobLevel { get; set; }
}

// Update DTOs (what the frontend sends to update)
public class UpdateRequisitionDto
{
    public string? Title { get; set; }
    public string? Department { get; set; }
    public string? JobLevel { get; set; }
    public string? Status { get; set; }
}
