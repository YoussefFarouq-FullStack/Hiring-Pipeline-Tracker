namespace HiringPipelineAPI.Data;

/// <summary>
/// Configuration for database seeding
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Sample candidate data for development
    /// </summary>
    public static class Candidates
    {
        public static readonly CandidateData[] SampleData = new[]
        {
            new CandidateData
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@email.com",
                Phone = "+1-555-0101",
                LinkedInUrl = "https://linkedin.com/in/johnsmith",
                Source = "LinkedIn",
                Status = "Applied",
                DaysAgo = 30
            },
            new CandidateData
            {
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "sarah.johnson@email.com",
                Phone = "+1-555-0102",
                LinkedInUrl = "https://linkedin.com/in/sarahjohnson",
                Source = "Indeed",
                Status = "Interviewing",
                DaysAgo = 28
            },
            new CandidateData
            {
                FirstName = "Michael",
                LastName = "Brown",
                Email = "michael.brown@email.com",
                Phone = "+1-555-0103",
                LinkedInUrl = "https://linkedin.com/in/michaelbrown",
                Source = "Company Website",
                Status = "Offered",
                DaysAgo = 25
            },
            new CandidateData
            {
                FirstName = "Emily",
                LastName = "Davis",
                Email = "emily.davis@email.com",
                Phone = "+1-555-0104",
                LinkedInUrl = "https://linkedin.com/in/emilydavis",
                Source = "Referral",
                Status = "Hired",
                DaysAgo = 35
            },
            new CandidateData
            {
                FirstName = "David",
                LastName = "Wilson",
                Email = "david.wilson@email.com",
                Phone = "+1-555-0105",
                LinkedInUrl = "https://linkedin.com/in/davidwilson",
                Source = "LinkedIn",
                Status = "Rejected",
                DaysAgo = 22
            },
            new CandidateData
            {
                FirstName = "Lisa",
                LastName = "Anderson",
                Email = "lisa.anderson@email.com",
                Phone = "+1-555-0106",
                LinkedInUrl = "https://linkedin.com/in/lisaanderson",
                Source = "Glassdoor",
                Status = "Applied",
                DaysAgo = 15
            },
            new CandidateData
            {
                FirstName = "Robert",
                LastName = "Taylor",
                Email = "robert.taylor@email.com",
                Phone = "+1-555-0107",
                LinkedInUrl = "https://linkedin.com/in/roberttaylor",
                Source = "Referral",
                Status = "Interviewing",
                DaysAgo = 20
            },
            new CandidateData
            {
                FirstName = "Jennifer",
                LastName = "Martinez",
                Email = "jennifer.martinez@email.com",
                Phone = "+1-555-0108",
                LinkedInUrl = "https://linkedin.com/in/jennifermartinez",
                Source = "LinkedIn",
                Status = "On Hold",
                DaysAgo = 18
            },
            new CandidateData
            {
                FirstName = "Christopher",
                LastName = "Garcia",
                Email = "chris.garcia@email.com",
                Phone = "+1-555-0109",
                LinkedInUrl = "https://linkedin.com/in/chrisgarcia",
                Source = "Indeed",
                Status = "Applied",
                DaysAgo = 12
            },
            new CandidateData
            {
                FirstName = "Amanda",
                LastName = "Rodriguez",
                Email = "amanda.rodriguez@email.com",
                Phone = "+1-555-0110",
                LinkedInUrl = "https://linkedin.com/in/amandarodriguez",
                Source = "Company Website",
                Status = "Phone Screen",
                DaysAgo = 10
            }
        };
    }

    /// <summary>
    /// Sample requisition data for development
    /// </summary>
    public static class Requisitions
    {
        public static readonly RequisitionData[] SampleData = new[]
        {
            new RequisitionData
            {
                Title = "Senior Software Engineer",
                Department = "Engineering",
                JobLevel = "Senior",
                Status = "Open",
                DaysAgo = 40
            },
            new RequisitionData
            {
                Title = "Product Manager",
                Department = "Product",
                JobLevel = "Mid",
                Status = "Open",
                DaysAgo = 35
            },
            new RequisitionData
            {
                Title = "UX Designer",
                Department = "Design",
                JobLevel = "Junior",
                Status = "Open",
                DaysAgo = 30
            },
            new RequisitionData
            {
                Title = "DevOps Engineer",
                Department = "Engineering",
                JobLevel = "Senior",
                Status = "On Hold",
                DaysAgo = 25
            },
            new RequisitionData
            {
                Title = "Data Scientist",
                Department = "Analytics",
                JobLevel = "Mid",
                Status = "Closed",
                DaysAgo = 45
            },
            new RequisitionData
            {
                Title = "Frontend Developer",
                Department = "Engineering",
                JobLevel = "Mid",
                Status = "Open",
                DaysAgo = 20
            },
            new RequisitionData
            {
                Title = "QA Engineer",
                Department = "Engineering",
                JobLevel = "Junior",
                Status = "Open",
                DaysAgo = 15
            }
        };
    }

    /// <summary>
    /// Sample application scenarios for development
    /// </summary>
    public static class ApplicationScenarios
    {
        public static readonly ApplicationScenarioData[] SampleData = new[]
        {
            new ApplicationScenarioData
            {
                CandidateIndex = 0,
                RequisitionIndex = 0,
                CurrentStage = "Technical Interview",
                Status = "Active"
            },
            new ApplicationScenarioData
            {
                CandidateIndex = 1,
                RequisitionIndex = 1,
                CurrentStage = "Onsite Interview",
                Status = "Active"
            },
            new ApplicationScenarioData
            {
                CandidateIndex = 2,
                RequisitionIndex = 0,
                CurrentStage = "Offer",
                Status = "Active"
            },
            new ApplicationScenarioData
            {
                CandidateIndex = 3,
                RequisitionIndex = 2,
                CurrentStage = "Hired",
                Status = "Hired"
            },
            new ApplicationScenarioData
            {
                CandidateIndex = 4,
                RequisitionIndex = 0,
                CurrentStage = "Rejected",
                Status = "Rejected"
            },
            new ApplicationScenarioData
            {
                CandidateIndex = 5,
                RequisitionIndex = 1,
                CurrentStage = "Phone Screen",
                Status = "Active"
            },
            new ApplicationScenarioData
            {
                CandidateIndex = 6,
                RequisitionIndex = 3,
                CurrentStage = "Technical Interview",
                Status = "Active"
            },
            new ApplicationScenarioData
            {
                CandidateIndex = 7,
                RequisitionIndex = 4,
                CurrentStage = "Applied",
                Status = "On Hold"
            },
            new ApplicationScenarioData
            {
                CandidateIndex = 8,
                RequisitionIndex = 5,
                CurrentStage = "Phone Screen",
                Status = "Active"
            },
            new ApplicationScenarioData
            {
                CandidateIndex = 9,
                RequisitionIndex = 6,
                CurrentStage = "Applied",
                Status = "Active"
            }
        };
    }

    /// <summary>
    /// Available stages for applications
    /// </summary>
    public static class Stages
    {
        public static readonly string[] AllStages = new[]
        {
            "Applied",
            "Phone Screen",
            "Technical Interview",
            "Onsite Interview",
            "Reference Check",
            "Offer",
            "Hired",
            "Rejected"
        };

        public static readonly Dictionary<string, StageInfo> StageDetails = new()
        {
            ["Phone Screen"] = new StageInfo { MovedBy = "HR Team", Notes = "Initial screening completed successfully" },
            ["Technical Interview"] = new StageInfo { MovedBy = "Hiring Manager", Notes = "Technical skills assessment passed" },
            ["Onsite Interview"] = new StageInfo { MovedBy = "Interview Panel", Notes = "Team fit and culture alignment confirmed" },
            ["Reference Check"] = new StageInfo { MovedBy = "HR Team", Notes = "References provided positive feedback" },
            ["Offer"] = new StageInfo { MovedBy = "Hiring Manager", Notes = "Offer extended and accepted" },
            ["Hired"] = new StageInfo { MovedBy = "HR Team", Notes = "Candidate successfully onboarded" },
            ["Rejected"] = new StageInfo { MovedBy = "Hiring Manager", Notes = "Candidate did not meet requirements" }
        };
    }

    /// <summary>
    /// Available sources for candidates
    /// </summary>
    public static class Sources
    {
        public static readonly string[] AllSources = new[]
        {
            "LinkedIn",
            "Indeed",
            "Company Website",
            "Referral",
            "Glassdoor",
            "Career Fair",
            "University Recruiting",
            "Job Board"
        };
    }

    /// <summary>
    /// Available departments
    /// </summary>
    public static class Departments
    {
        public static readonly string[] AllDepartments = new[]
        {
            "Engineering",
            "Product",
            "Design",
            "Analytics",
            "Marketing",
            "Sales",
            "HR",
            "Finance",
            "Operations"
        };
    }

    /// <summary>
    /// Available job levels
    /// </summary>
    public static class JobLevels
    {
        public static readonly string[] AllLevels = new[]
        {
            "Entry",
            "Junior",
            "Mid",
            "Senior",
            "Lead",
            "Principal",
            "Director",
            "VP",
            "C-Level"
        };
    }
}

/// <summary>
/// Data structure for candidate seeding
/// </summary>
public class CandidateData
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int DaysAgo { get; set; }
}

/// <summary>
/// Data structure for requisition seeding
/// </summary>
public class RequisitionData
{
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int DaysAgo { get; set; }
}

/// <summary>
/// Data structure for application scenario seeding
/// </summary>
public class ApplicationScenarioData
{
    public int CandidateIndex { get; set; }
    public int RequisitionIndex { get; set; }
    public string CurrentStage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Information about a stage transition
/// </summary>
public class StageInfo
{
    public string MovedBy { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
