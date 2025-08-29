namespace HiringPipelineAPI.DTOs;

// Common DTOs used across multiple entities to avoid duplication
public class CandidateSummaryDto
{
    public int CandidateId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class RequisitionSummaryDto
{
    public int RequisitionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? JobLevel { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ApplicationSummaryDto
{
    public int ApplicationId { get; set; }
    public string CurrentStage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class StageHistorySummaryDto
{
    public int StageHistoryId { get; set; }
    public string? FromStage { get; set; }
    public string ToStage { get; set; } = string.Empty;
    public string MovedBy { get; set; } = string.Empty;
    public DateTime MovedAt { get; set; }
}
