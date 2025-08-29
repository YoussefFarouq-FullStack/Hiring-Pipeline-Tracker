namespace HiringPipelineAPI.DTOs;

// Response DTOs (what the frontend receives)
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
