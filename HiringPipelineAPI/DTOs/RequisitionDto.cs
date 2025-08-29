namespace HiringPipelineAPI.DTOs;

// Response DTOs (what the frontend receives)
public class RequisitionDto
{
    public int RequisitionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? JobLevel { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class RequisitionDetailDto : RequisitionDto
{
    public int ApplicationCount { get; set; }
    public List<ApplicationSummaryDto> Applications { get; set; } = new();
}

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
