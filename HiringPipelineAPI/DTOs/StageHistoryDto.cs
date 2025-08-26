namespace HiringPipelineAPI.DTOs;

public class CreateStageHistoryDto
{
    public int ApplicationId { get; set; }
    public string? FromStage { get; set; }
    public string ToStage { get; set; } = string.Empty;
    public string MovedBy { get; set; } = string.Empty;
}
