namespace HiringPipelineCore.DTOs;

// Core business DTOs for Application entity
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
