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
        public const int MaxResume = 500;
        public const int MaxFileName = 255;
        public const int MaxFilePath = 500;
        public const int MaxDescription = 2000;
        public const int MaxSkills = 1000;
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

    /// <summary>
    /// Valid job levels for requisitions
    /// </summary>
    public static class JobLevels
    {
        /// <summary>
        /// Array of valid job levels
        /// </summary>
        public static readonly string[] ValidLevels = 
        {
            "Entry", "Junior", "Mid", "Senior", "Lead", "Principal", "Director", "VP", "C-Level"
        };
    }
}
