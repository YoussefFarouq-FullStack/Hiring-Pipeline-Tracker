namespace HiringPipelineCore.DTOs;

// Core business DTOs for Requisition entity
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

// Summary DTOs for use across the domain
public class RequisitionSummaryDto
{
    public int RequisitionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? JobLevel { get; set; }
    public string Status { get; set; } = string.Empty;
}
