namespace HiringPipelineAPI.Models;

public class ValidationErrorResponse
{
    public string Type { get; set; } = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
    public string Title { get; set; } = "One or more validation errors occurred.";
    public int Status { get; set; } = 400;
    public string TraceId { get; set; } = string.Empty;
    public Dictionary<string, string[]> Errors { get; set; } = new();
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
