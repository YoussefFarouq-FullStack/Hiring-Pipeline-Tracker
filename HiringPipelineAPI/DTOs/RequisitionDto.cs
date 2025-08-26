namespace HiringPipelineAPI.DTOs;

public class CreateRequisitionDto
{
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? JobLevel { get; set; }
}

public class UpdateRequisitionDto
{
    public string? Title { get; set; }
    public string? Department { get; set; }
    public string? JobLevel { get; set; }
    public string? Status { get; set; }
}
