namespace HiringPipelineAPI.Models;

public class StageHistory
{
    public int StageHistoryId { get; set; }

    // Foreign Key
    public int ApplicationId { get; set; }
    public Application Application { get; set; } = null!;

    public string? FromStage { get; set; }
    public string ToStage { get; set; } = string.Empty;
    public string MovedBy { get; set; } = string.Empty;
    public DateTime MovedAt { get; set; } = DateTime.UtcNow;
}
