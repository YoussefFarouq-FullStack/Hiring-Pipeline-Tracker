using System.Text.Json.Serialization;

namespace HiringPipelineAPI.Models;

public class Requisition
{
    public int RequisitionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? JobLevel { get; set; }
    public string Status { get; set; } = "Open"; // Open, Closed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [JsonIgnore]
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
