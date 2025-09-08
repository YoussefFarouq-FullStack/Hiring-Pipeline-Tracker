namespace HiringPipelineAPI.DTOs;

// API-specific DTOs for StageHistory operations
// These are only used by controllers and not shared with Core

// Create DTOs (what the frontend sends to create)
public class CreateStageHistoryDto
{
    public int ApplicationId { get; set; }
    public string? FromStage { get; set; }
    public string ToStage { get; set; } = string.Empty;
    public string MovedBy { get; set; } = string.Empty;
}
