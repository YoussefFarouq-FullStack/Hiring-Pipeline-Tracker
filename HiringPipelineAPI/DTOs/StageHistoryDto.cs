namespace HiringPipelineAPI.DTOs;

// Response DTOs (what the frontend receives)
public class StageHistoryDto
{
    public int StageHistoryId { get; set; }
    public int ApplicationId { get; set; }
    public string? FromStage { get; set; }
    public string ToStage { get; set; } = string.Empty;
    public string MovedBy { get; set; } = string.Empty;
    public DateTime MovedAt { get; set; }
}

public class StageHistoryDetailDto : StageHistoryDto
{
    public ApplicationSummaryDto Application { get; set; } = new();
}

// Create DTOs (what the frontend sends to create)
public class CreateStageHistoryDto
{
    public int ApplicationId { get; set; }
    public string? FromStage { get; set; }
    public string ToStage { get; set; } = string.Empty;
    public string MovedBy { get; set; } = string.Empty;
}
