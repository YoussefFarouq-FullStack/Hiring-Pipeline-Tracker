namespace HiringPipelineInfrastructure.Data;

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
                Resume = "https://example.com/resumes/john-smith.pdf",
                Skills = "C#, .NET, SQL Server, Azure, JavaScript",
                Status = "Applied",
                DaysAgo = 30
            },
            new CandidateData
            {
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "sarah.johnson@email.com",
                Phone = "+1-555-0102",
                Resume = "https://example.com/resumes/sarah-johnson.pdf",
                Skills = "React, TypeScript, Node.js, MongoDB, AWS",
                Status = "Interviewing",
                DaysAgo = 28
            },
            new CandidateData
            {
                FirstName = "Michael",
                LastName = "Brown",
                Email = "michael.brown@email.com",
                Phone = "+1-555-0103",
                Resume = "https://example.com/resumes/michael-brown.pdf",
                Skills = "Python, Django, PostgreSQL, Docker, Kubernetes",
                Status = "Offered",
                DaysAgo = 25
            },
            new CandidateData
            {
                FirstName = "Emily",
                LastName = "Davis",
                Email = "emily.davis@email.com",
                Phone = "+1-555-0104",
                Resume = "https://example.com/resumes/emily-davis.pdf",
                Skills = "Java, Spring Boot, MySQL, Redis, Microservices",
                Status = "Hired",
                DaysAgo = 35
            },
            new CandidateData
            {
                FirstName = "David",
                LastName = "Wilson",
                Email = "david.wilson@email.com",
                Phone = "+1-555-0105",
                Resume = "https://example.com/resumes/david-wilson.pdf",
                Skills = "Angular, RxJS, Firebase, GraphQL, Jest",
                Status = "Rejected",
                DaysAgo = 22
            },
            new CandidateData
            {
                FirstName = "Lisa",
                LastName = "Anderson",
                Email = "lisa.anderson@email.com",
                Phone = "+1-555-0106",
                Resume = "https://example.com/resumes/lisa-anderson.pdf",
                Skills = "Vue.js, Nuxt.js, Tailwind CSS, Supabase, Cypress",
                Status = "Applied",
                DaysAgo = 15
            },
            new CandidateData
            {
                FirstName = "Robert",
                LastName = "Taylor",
                Email = "robert.taylor@email.com",
                Phone = "+1-555-0107",
                Resume = "https://example.com/resumes/robert-taylor.pdf",
                Skills = "Go, Gin, PostgreSQL, Docker, Terraform",
                Status = "Interviewing",
                DaysAgo = 20
            },
            new CandidateData
            {
                FirstName = "Jennifer",
                LastName = "Martinez",
                Email = "jennifer.martinez@email.com",
                Phone = "+1-555-0108",
                Resume = "https://example.com/resumes/jennifer-martinez.pdf",
                Skills = "PHP, Laravel, MySQL, Redis, Vue.js",
                Status = "On Hold",
                DaysAgo = 18
            },
            new CandidateData
            {
                FirstName = "Christopher",
                LastName = "Garcia",
                Email = "chris.garcia@email.com",
                Phone = "+1-555-0109",
                Resume = "https://example.com/resumes/christopher-garcia.pdf",
                Skills = "Ruby, Rails, PostgreSQL, Sidekiq, RSpec",
                Status = "Applied",
                DaysAgo = 12
            },
            new CandidateData
            {
                FirstName = "Amanda",
                LastName = "Rodriguez",
                Email = "amanda.rodriguez@email.com",
                Phone = "+1-555-0110",
                Resume = "https://example.com/resumes/amanda-rodriguez.pdf",
                Skills = "Swift, iOS, Core Data, Alamofire, XCTest",
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
                Description = "Lead development of scalable web applications using modern technologies. Mentor junior developers and drive technical decisions.",
                Department = "Engineering",
                Location = "San Francisco, CA (Hybrid)",
                EmploymentType = "Full-time",
                Salary = "$120,000 - $160,000",
                IsDraft = false,
                Priority = "High",
                RequiredSkills = "C#, .NET Core, SQL Server, Azure, Microservices, REST APIs",
                ExperienceLevel = "Senior",
                JobLevel = "Senior",
                Status = "Open",
                DaysAgo = 40
            },
            new RequisitionData
            {
                Title = "Product Manager",
                Description = "Define product strategy and roadmap for our core platform. Work closely with engineering and design teams to deliver user-focused features.",
                Department = "Product",
                Location = "New York, NY (Remote)",
                EmploymentType = "Full-time",
                Salary = "$100,000 - $140,000",
                IsDraft = false,
                Priority = "High",
                RequiredSkills = "Product Strategy, Agile, User Research, Analytics, Stakeholder Management",
                ExperienceLevel = "Mid",
                JobLevel = "Mid",
                Status = "Open",
                DaysAgo = 35
            },
            new RequisitionData
            {
                Title = "UX Designer",
                Description = "Create intuitive user experiences for our web and mobile applications. Conduct user research and collaborate with product and engineering teams.",
                Department = "Design",
                Location = "Austin, TX (Hybrid)",
                EmploymentType = "Full-time",
                Salary = "$70,000 - $90,000",
                IsDraft = false,
                Priority = "Medium",
                RequiredSkills = "Figma, User Research, Prototyping, Design Systems, Accessibility",
                ExperienceLevel = "Junior",
                JobLevel = "Junior",
                Status = "Open",
                DaysAgo = 30
            },
            new RequisitionData
            {
                Title = "DevOps Engineer",
                Description = "Build and maintain CI/CD pipelines, manage cloud infrastructure, and ensure system reliability and scalability.",
                Department = "Engineering",
                Location = "Seattle, WA (Remote)",
                EmploymentType = "Full-time",
                Salary = "$110,000 - $150,000",
                IsDraft = true,
                Priority = "Medium",
                RequiredSkills = "Docker, Kubernetes, AWS, Terraform, CI/CD, Monitoring",
                ExperienceLevel = "Senior",
                JobLevel = "Senior",
                Status = "On Hold",
                DaysAgo = 25
            },
            new RequisitionData
            {
                Title = "Data Scientist",
                Description = "Analyze large datasets to extract insights and build predictive models. Work with stakeholders to solve business problems using data.",
                Department = "Analytics",
                Location = "Boston, MA (Hybrid)",
                EmploymentType = "Full-time",
                Salary = "$95,000 - $130,000",
                IsDraft = false,
                Priority = "Low",
                RequiredSkills = "Python, R, SQL, Machine Learning, Statistics, Data Visualization",
                ExperienceLevel = "Mid",
                JobLevel = "Mid",
                Status = "Closed",
                DaysAgo = 45
            },
            new RequisitionData
            {
                Title = "Frontend Developer",
                Description = "Build responsive and interactive user interfaces using modern frontend technologies. Collaborate with UX designers and backend developers.",
                Department = "Engineering",
                Location = "Chicago, IL (Hybrid)",
                EmploymentType = "Full-time",
                Salary = "$80,000 - $110,000",
                IsDraft = false,
                Priority = "High",
                RequiredSkills = "React, TypeScript, HTML/CSS, JavaScript, REST APIs, Testing",
                ExperienceLevel = "Mid",
                JobLevel = "Mid",
                Status = "Open",
                DaysAgo = 20
            },
            new RequisitionData
            {
                Title = "QA Engineer",
                Description = "Design and execute comprehensive test plans for web applications. Work with development teams to ensure quality and reliability.",
                Department = "Engineering",
                Location = "Denver, CO (Hybrid)",
                EmploymentType = "Full-time",
                Salary = "$65,000 - $85,000",
                IsDraft = false,
                Priority = "Medium",
                RequiredSkills = "Manual Testing, Automated Testing, Selenium, Test Planning, Bug Tracking",
                ExperienceLevel = "Junior",
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
    public string Resume { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int DaysAgo { get; set; }
}

/// <summary>
/// Data structure for requisition seeding
/// </summary>
public class RequisitionData
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Department { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public string? Salary { get; set; }
    public bool IsDraft { get; set; } = false;
    public string Priority { get; set; } = "Medium";
    public string? RequiredSkills { get; set; }
    public string? ExperienceLevel { get; set; }
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
