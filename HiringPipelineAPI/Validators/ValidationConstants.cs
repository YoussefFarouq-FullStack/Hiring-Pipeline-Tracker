namespace HiringPipelineAPI.Validators;

public static class ValidationConstants
{
    // Common validation rules
    public static class Lengths
    {
        public const int MinName = 1;
        public const int MaxName = 50;
        public const int MaxEmail = 100;
        public const int MaxPhone = 20;
        public const int MaxLinkedInUrl = 200;
        public const int MaxSource = 100;
        public const int MaxStatus = 50;
        public const int MaxTitle = 100;
        public const int MaxDepartment = 100;
        public const int MaxJobLevel = 50;
        public const int MaxStage = 50;
        public const int MaxMovedBy = 100;
    }

    public static class RegexPatterns
    {
        public const string Name = @"^[a-zA-Z\s\-']+$";
        public const string Title = @"^[a-zA-Z0-9\s\-&,\.]+$";
        public const string Department = @"^[a-zA-Z\s\-&]+$";
        public const string Phone = @"^[\+]?[0-9\s\-\(\)]+$";
        public const string MovedBy = @"^[a-zA-Z\s\-\.]+$";
    }

    public static class CandidateStatuses
    {
        public static readonly string[] ValidStatuses = 
        {
            "Applied", "Interviewing", "Offered", "Hired", "Rejected", "Withdrawn"
        };
    }

    public static class ApplicationStages
    {
        public static readonly string[] ValidStages = 
        {
            "Applied", "Phone Screen", "Technical Interview", "Onsite Interview", 
            "Reference Check", "Offer", "Hired", "Rejected"
        };
    }

    public static class ApplicationStatuses
    {
        public static readonly string[] ValidStatuses = 
        {
            "Active", "On Hold", "Withdrawn", "Rejected", "Hired"
        };
    }

    public static class RequisitionStatuses
    {
        public static readonly string[] ValidStatuses = 
        {
            "Open", "On Hold", "Closed", "Cancelled"
        };
    }

    public static class JobLevels
    {
        public static readonly string[] ValidLevels = 
        {
            "Entry", "Junior", "Mid", "Senior", "Lead", "Principal", "Director", "VP", "C-Level"
        };
    }

    public static class LinkedInUrlPatterns
    {
        public static readonly string[] ValidPatterns = 
        {
            "https://www.linkedin.com/",
            "http://www.linkedin.com/",
            "https://linkedin.com/",
            "http://linkedin.com/"
        };
    }
}
