using Microsoft.EntityFrameworkCore;
using HiringPipelineCore.Entities;
using BCrypt.Net;

namespace HiringPipelineInfrastructure.Data;

/// <summary>
/// Database initializer for seeding test data during development
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Seeds the database with initial test data
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="isDevelopment">Whether we're in development mode</param>
    public static async Task SeedAsync(HiringPipelineDbContext context, bool isDevelopment = true)
    {
        if (!isDevelopment)
            return;

        try
        {
            // Check if database exists, if not create it
            if (!await context.Database.CanConnectAsync())
            {
                Console.WriteLine("Database does not exist. Creating database...");
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                Console.WriteLine("Database already exists. Skipping database creation.");
            }

            // Always seed users for development
            Console.WriteLine("Starting database seeding...");

            Console.WriteLine("Seeding database with initial test data...");

            // Seed roles and permissions first
            await SeedRolesAndPermissionsAsync(context);
            
            // Seed candidates
            var candidates = await SeedCandidatesAsync(context);
            
            // Seed requisitions
            var requisitions = await SeedRequisitionsAsync(context);
            
            // Seed applications
            var applications = await SeedApplicationsAsync(context, candidates, requisitions);
            
            // Seed stage history
            await SeedStageHistoryAsync(context, applications);
            
            // Seed users
            await SeedUsersAsync(context);

            await context.SaveChangesAsync();
            
            Console.WriteLine($"Database seeded successfully with roles, permissions, {candidates.Count} candidates, {requisitions.Count} requisitions, {applications.Count} applications, and stage history.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding database: {ex.Message}");
            throw;
        }
    }

    private static async Task<List<Candidate>> SeedCandidatesAsync(HiringPipelineDbContext context)
    {
        // Only seed candidates if none exist
        if (await context.Candidates.AnyAsync())
        {
            Console.WriteLine("Candidates already exist. Skipping candidate seeding.");
            return await context.Candidates.ToListAsync();
        }

        var candidates = new List<Candidate>();

        foreach (var candidateData in SeedData.Candidates.SampleData)
        {
            var candidate = new Candidate
            {
                FirstName = candidateData.FirstName,
                LastName = candidateData.LastName,
                Email = candidateData.Email,
                Phone = candidateData.Phone,
                ResumeFileName = candidateData.Resume,
                ResumeFilePath = candidateData.Resume,
                Description = $"Sample candidate with {candidateData.Skills} skills",
                Skills = candidateData.Skills,
                Status = candidateData.Status,
                CreatedAt = DateTime.UtcNow.AddDays(-candidateData.DaysAgo),
                UpdatedAt = DateTime.UtcNow.AddDays(-candidateData.DaysAgo + 5) // Updated 5 days after creation
            };

            candidates.Add(candidate);
        }

        await context.Candidates.AddRangeAsync(candidates);
        await context.SaveChangesAsync();
        
        return candidates;
    }

    private static async Task<List<Requisition>> SeedRequisitionsAsync(HiringPipelineDbContext context)
    {
        var requisitions = new List<Requisition>();

        foreach (var requisitionData in SeedData.Requisitions.SampleData)
        {
            var requisition = new Requisition
            {
                Title = requisitionData.Title,
                Description = requisitionData.Description,
                Department = requisitionData.Department,
                Location = requisitionData.Location,
                EmploymentType = requisitionData.EmploymentType,
                Salary = requisitionData.Salary,
                IsDraft = requisitionData.IsDraft,
                Priority = requisitionData.Priority,
                RequiredSkills = requisitionData.RequiredSkills,
                ExperienceLevel = requisitionData.ExperienceLevel,
                JobLevel = requisitionData.JobLevel,
                Status = requisitionData.Status,
                CreatedAt = DateTime.UtcNow.AddDays(-requisitionData.DaysAgo),
                UpdatedAt = DateTime.UtcNow.AddDays(-requisitionData.DaysAgo)
            };

            requisitions.Add(requisition);
        }

        await context.Requisitions.AddRangeAsync(requisitions);
        await context.SaveChangesAsync();
        
        return requisitions;
    }

    private static async Task<List<Application>> SeedApplicationsAsync(HiringPipelineDbContext context, List<Candidate> candidates, List<Requisition> requisitions)
    {
        var applications = new List<Application>();

        foreach (var scenario in SeedData.ApplicationScenarios.SampleData)
        {
            if (scenario.CandidateIndex < candidates.Count && scenario.RequisitionIndex < requisitions.Count)
            {
                var candidate = candidates[scenario.CandidateIndex];
                var requisition = requisitions[scenario.RequisitionIndex];

                var application = new Application
                {
                    CandidateId = candidate.CandidateId,
                    RequisitionId = requisition.RequisitionId,
                    CurrentStage = scenario.CurrentStage,
                    Status = scenario.Status,
                    CreatedAt = candidate.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                };

                applications.Add(application);
            }
        }

        await context.Applications.AddRangeAsync(applications);
        await context.SaveChangesAsync();
        
        return applications;
    }

    private static async Task SeedStageHistoryAsync(HiringPipelineDbContext context, List<Application> applications)
    {
        var stageHistory = new List<StageHistory>();
        var random = new Random();

        foreach (var application in applications)
        {
            var stages = GetStagesForApplication(application.CurrentStage);
            var currentDate = application.CreatedAt;

            foreach (var stage in stages)
            {
                var stageHistoryEntry = new StageHistory
                {
                    ApplicationId = application.ApplicationId,
                    FromStage = stage.FromStage,
                    ToStage = stage.ToStage,
                    MovedBy = stage.MovedBy,
                    MovedAt = currentDate
                };

                stageHistory.Add(stageHistoryEntry);
                currentDate = currentDate.AddDays(random.Next(1, 7)); // Random days between stages
            }
        }

        await context.StageHistories.AddRangeAsync(stageHistory);
        await context.SaveChangesAsync();
    }

    private static List<StageHistoryData> GetStagesForApplication(string currentStage)
    {
        var allStages = SeedData.Stages.AllStages;
        var stageHistory = new List<StageHistoryData>();
        
        var currentStageIndex = Array.IndexOf(allStages, currentStage);
        if (currentStageIndex == -1) currentStageIndex = 0;

        for (int i = 0; i <= currentStageIndex; i++)
        {
            if (i == 0)
            {
                stageHistory.Add(new StageHistoryData
                {
                    FromStage = null,
                    ToStage = allStages[i],
                    MovedBy = "System",
                    Notes = "Application submitted"
                });
            }
            else
            {
                var stageName = allStages[i];
                var stageInfo = SeedData.Stages.StageDetails.GetValueOrDefault(stageName, new StageInfo { MovedBy = "System", Notes = "Stage transition" });

                stageHistory.Add(new StageHistoryData
                {
                    FromStage = allStages[i - 1],
                    ToStage = stageName,
                    MovedBy = stageInfo.MovedBy,
                    Notes = stageInfo.Notes
                });
            }
        }

        return stageHistory;
    }

    private static async Task SeedRolesAndPermissionsAsync(HiringPipelineDbContext context)
    {
        // Only seed if no roles exist
        if (await context.Roles.AnyAsync())
        {
            Console.WriteLine("Roles and permissions already exist. Skipping role seeding.");
            return;
        }

        Console.WriteLine("Seeding roles and permissions...");

        // Seed permissions first
        var permissions = RoleSeedData.GetDefaultPermissions();
        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        // Seed roles
        var roles = RoleSeedData.GetDefaultRoles();
        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        // Seed role permissions using names instead of IDs
        await SeedRolePermissionsAsync(context);

        Console.WriteLine($"Seeded {roles.Count} roles and {permissions.Count} permissions.");
    }

    private static async Task SeedRolePermissionsAsync(HiringPipelineDbContext context)
    {
        var rolePermissions = new List<RolePermission>();

        // Get roles and permissions by name
        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
        var recruiterRole = await context.Roles.FirstAsync(r => r.Name == "Recruiter");
        var hiringManagerRole = await context.Roles.FirstAsync(r => r.Name == "Hiring Manager");
        var interviewerRole = await context.Roles.FirstAsync(r => r.Name == "Interviewer");
        var readOnlyRole = await context.Roles.FirstAsync(r => r.Name == "Read-only");

        var permissions = await context.Permissions.ToListAsync();

        // Admin - All permissions
        foreach (var permission in permissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id,
                AssignedAt = DateTime.UtcNow
            });
        }

        // Recruiter - Requisition and Candidate management
        var recruiterPermissions = permissions.Where(p => 
            p.Resource == "Requisition" || 
            p.Resource == "Candidate" || 
            p.Resource == "Application" ||
            p.Resource == "StageHistory").ToList();

        foreach (var permission in recruiterPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = recruiterRole.Id,
                PermissionId = permission.Id,
                AssignedAt = DateTime.UtcNow
            });
        }

        // Hiring Manager - Review and feedback
        var hiringManagerPermissions = permissions.Where(p => 
            p.Name == "ViewRequisition" ||
            p.Name == "ViewCandidate" ||
            p.Name == "MoveStage" ||
            p.Name == "RejectApplication" ||
            p.Name == "HireCandidate" ||
            p.Name == "ViewApplication" ||
            p.Name == "AddFeedback" ||
            p.Name == "ViewStageHistory").ToList();

        foreach (var permission in hiringManagerPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = hiringManagerRole.Id,
                PermissionId = permission.Id,
                AssignedAt = DateTime.UtcNow
            });
        }

        // Interviewer - Limited access
        var interviewerPermissions = permissions.Where(p => 
            p.Name == "ViewCandidate" ||
            p.Name == "ViewApplication" ||
            p.Name == "AddFeedback" ||
            p.Name == "ViewStageHistory").ToList();

        foreach (var permission in interviewerPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = interviewerRole.Id,
                PermissionId = permission.Id,
                AssignedAt = DateTime.UtcNow
            });
        }

        // Read-only - View only
        var readOnlyPermissions = permissions.Where(p => 
            p.Name == "ViewRequisition" ||
            p.Name == "ViewCandidate" ||
            p.Name == "ViewApplication" ||
            p.Name == "ViewStageHistory").ToList();

        foreach (var permission in readOnlyPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = readOnlyRole.Id,
                PermissionId = permission.Id,
                AssignedAt = DateTime.UtcNow
            });
        }

        await context.RolePermissions.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(HiringPipelineDbContext context)
    {
        var usersExist = await context.Users.AnyAsync();
        
        if (!usersExist)
        {
            Console.WriteLine("Seeding default users...");

            // Seed users using the new UserSeedData
            var users = UserSeedData.GetDefaultUsers();
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            Console.WriteLine($"Seeded {users.Count} default users.");
        }
        else
        {
            Console.WriteLine("Users already exist. Skipping user creation.");
        }

        // Always ensure role assignments (even if users already exist)
        await AssignRolesToUsersAsync(context);
    }

    private static async Task AssignRolesToUsersAsync(HiringPipelineDbContext context)
    {
        Console.WriteLine("Ensuring role assignments for users...");

        // Get users and roles by name
        var adminUser = await context.Users.FirstAsync(u => u.Username == "admin");
        var recruiterUser = await context.Users.FirstAsync(u => u.Username == "recruiter1");
        var hiringManagerUser = await context.Users.FirstAsync(u => u.Username == "hiringmanager1");
        var interviewerUser = await context.Users.FirstAsync(u => u.Username == "interviewer1");
        var readOnlyUser = await context.Users.FirstAsync(u => u.Username == "readonly1");

        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
        var recruiterRole = await context.Roles.FirstAsync(r => r.Name == "Recruiter");
        var hiringManagerRole = await context.Roles.FirstAsync(r => r.Name == "Hiring Manager");
        var interviewerRole = await context.Roles.FirstAsync(r => r.Name == "Interviewer");
        var readOnlyRole = await context.Roles.FirstAsync(r => r.Name == "Read-only");

        // Check and assign roles only if they don't already exist
        var userRoleAssignments = new[]
        {
            new { User = adminUser, Role = adminRole, UserName = "admin" },
            new { User = recruiterUser, Role = recruiterRole, UserName = "recruiter1" },
            new { User = hiringManagerUser, Role = hiringManagerRole, UserName = "hiringmanager1" },
            new { User = interviewerUser, Role = interviewerRole, UserName = "interviewer1" },
            new { User = readOnlyUser, Role = readOnlyRole, UserName = "readonly1" }
        };

        foreach (var assignment in userRoleAssignments)
        {
            var existingUserRole = await context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == assignment.User.Id && ur.RoleId == assignment.Role.Id);

            if (existingUserRole == null)
            {
                var userRole = new UserRole 
                { 
                    UserId = assignment.User.Id, 
                    RoleId = assignment.Role.Id, 
                    AssignedAt = DateTime.UtcNow 
                };
                await context.UserRoles.AddAsync(userRole);
                Console.WriteLine($"Assigned {assignment.Role.Name} role to {assignment.UserName}");
            }
            else
            {
                Console.WriteLine($"User {assignment.UserName} already has {assignment.Role.Name} role");
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine("Role assignments completed.");
    }

    /// <summary>
    /// Seeds the database with customizable amounts of test data
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="request">Request specifying what data to create and how much</param>
    /// <returns>Summary of what was created</returns>
    public static async Task<object> SeedCustomAsync(HiringPipelineDbContext context, object request)
    {
        var seedRequest = (dynamic)request;
        
        Console.WriteLine("Starting custom database seeding...");
        var result = new
        {
            CandidatesCreated = 0,
            RequisitionsCreated = 0,
            ApplicationsCreated = 0,
            StageHistoryCreated = 0,
            UsersCreated = 0
        };

        // Ensure roles and permissions exist first
        await SeedRolesAndPermissionsAsync(context);

        // Create candidates if requested
        if (seedRequest.CandidatesCount > 0)
        {
            Console.WriteLine($"Creating {seedRequest.CandidatesCount} candidates...");
            var candidates = await CreateCustomCandidatesAsync(context, (int)seedRequest.CandidatesCount);
            result = new { CandidatesCreated = candidates.Count(), RequisitionsCreated = result.RequisitionsCreated, ApplicationsCreated = result.ApplicationsCreated, StageHistoryCreated = result.StageHistoryCreated, UsersCreated = result.UsersCreated };
        }

        // Create requisitions if requested
        if (seedRequest.RequisitionsCount > 0)
        {
            Console.WriteLine($"Creating {seedRequest.RequisitionsCount} requisitions...");
            var requisitions = await CreateCustomRequisitionsAsync(context, (int)seedRequest.RequisitionsCount);
            result = new { CandidatesCreated = result.CandidatesCreated, RequisitionsCreated = requisitions.Count(), ApplicationsCreated = result.ApplicationsCreated, StageHistoryCreated = result.StageHistoryCreated, UsersCreated = result.UsersCreated };
        }

        // Create applications if requested
        if (seedRequest.ApplicationsCount > 0)
        {
            Console.WriteLine($"Creating {seedRequest.ApplicationsCount} applications...");
            var candidates = await context.Candidates.ToListAsync();
            var requisitions = await context.Requisitions.ToListAsync();
            
            if (candidates.Count == 0 || requisitions.Count == 0)
            {
                throw new InvalidOperationException("Cannot create applications without candidates and requisitions. Please create candidates and requisitions first.");
            }

            var applications = await CreateCustomApplicationsAsync(context, (int)seedRequest.ApplicationsCount, candidates, requisitions);
            result = new { CandidatesCreated = result.CandidatesCreated, RequisitionsCreated = result.RequisitionsCreated, ApplicationsCreated = applications.Count(), StageHistoryCreated = result.StageHistoryCreated, UsersCreated = result.UsersCreated };
        }

        // Create stage history if requested
        if (seedRequest.StageHistoryCount > 0)
        {
            Console.WriteLine($"Creating {seedRequest.StageHistoryCount} stage history entries...");
            var applications = await context.Applications.ToListAsync();
            
            if (applications.Count == 0)
            {
                throw new InvalidOperationException("Cannot create stage history without applications. Please create applications first.");
            }

            var stageHistory = await CreateCustomStageHistoryAsync(context, (int)seedRequest.StageHistoryCount, applications);
            result = new { CandidatesCreated = result.CandidatesCreated, RequisitionsCreated = result.RequisitionsCreated, ApplicationsCreated = result.ApplicationsCreated, StageHistoryCreated = stageHistory.Count(), UsersCreated = result.UsersCreated };
        }

        // Create additional users if requested
        if (seedRequest.CreateUsers)
        {
            Console.WriteLine("Creating additional test users...");
            var users = await CreateCustomUsersAsync(context);
            result = new { CandidatesCreated = result.CandidatesCreated, RequisitionsCreated = result.RequisitionsCreated, ApplicationsCreated = result.ApplicationsCreated, StageHistoryCreated = result.StageHistoryCreated, UsersCreated = users.Count() };
        }

        Console.WriteLine("Custom database seeding completed successfully!");
        return result;
    }

    private static async Task<List<Candidate>> CreateCustomCandidatesAsync(HiringPipelineDbContext context, int count)
    {
        var candidates = new List<Candidate>();
        var random = new Random();
        
        var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emily", "Chris", "Lisa", "Alex", "Maria", "James", "Anna", "Robert", "Emma", "Daniel", "Olivia", "Mark", "Sophia", "Paul", "Isabella" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin" };
        var skills = new[] { "C#", "JavaScript", "Python", "Java", "React", "Angular", "SQL", "Azure", "AWS", "Docker", "Kubernetes", "Git", "Node.js", "TypeScript", "HTML", "CSS" };
        var emails = new[] { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "company.com" };

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var email = $"{firstName.ToLower()}.{lastName.ToLower()}@{emails[random.Next(emails.Length)]}";
            
            var candidate = new Candidate
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = $"{random.Next(100, 999)}-{random.Next(100, 999)}-{random.Next(1000, 9999)}",
                Skills = string.Join(", ", skills.OrderBy(x => random.Next()).Take(random.Next(3, 8))),
                // Experience property removed - not in Candidate entity
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                UpdatedAt = DateTime.UtcNow
            };

            candidates.Add(candidate);
            await context.Candidates.AddAsync(candidate);
        }

        await context.SaveChangesAsync();
        return candidates;
    }

    private static async Task<List<Requisition>> CreateCustomRequisitionsAsync(HiringPipelineDbContext context, int count)
    {
        var requisitions = new List<Requisition>();
        var random = new Random();
        
        var positions = new[] { "Software Engineer", "Senior Software Engineer", "Frontend Developer", "Backend Developer", "Full Stack Developer", "DevOps Engineer", "QA Engineer", "Product Manager", "UX Designer", "Data Analyst", "Project Manager", "Technical Lead" };
        var departments = new[] { "Engineering", "Product", "Design", "Data Science", "Marketing", "Sales", "Operations" };
        var locations = new[] { "New York", "San Francisco", "Los Angeles", "Chicago", "Boston", "Seattle", "Austin", "Remote", "Hybrid" };

        for (int i = 0; i < count; i++)
        {
            var position = positions[random.Next(positions.Length)];
            var department = departments[random.Next(departments.Length)];
            var location = locations[random.Next(locations.Length)];
            
            var requisition = new Requisition
            {
                Title = position,
                Department = department,
                Location = location,
                Description = $"We are looking for a talented {position.ToLower()} to join our {department} team. This is a great opportunity to work on exciting projects and grow your career.",
                RequiredSkills = "Bachelor's degree in Computer Science or related field, 3+ years of experience, strong problem-solving skills, excellent communication abilities.",
                Status = random.Next(3) == 0 ? "Draft" : (random.Next(2) == 0 ? "Open" : "Closed"),
                Priority = random.Next(3) switch { 0 => "High", 1 => "Medium", _ => "Low" },
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 180)),
                UpdatedAt = DateTime.UtcNow
            };

            requisitions.Add(requisition);
            await context.Requisitions.AddAsync(requisition);
        }

        await context.SaveChangesAsync();
        return requisitions;
    }

    private static async Task<List<Application>> CreateCustomApplicationsAsync(HiringPipelineDbContext context, int count, List<Candidate> candidates, List<Requisition> requisitions)
    {
        var applications = new List<Application>();
        var random = new Random();
        
        var stages = new[] { "Applied", "Phone Screen", "Technical Interview", "Onsite Interview", "Reference Check", "Offer", "Hired", "Rejected" };
        var statuses = new[] { "Active", "In Progress", "Completed", "Withdrawn" };

        for (int i = 0; i < count; i++)
        {
            var candidate = candidates[random.Next(candidates.Count)];
            var requisition = requisitions[random.Next(requisitions.Count)];
            var stage = stages[random.Next(stages.Length)];
            var status = statuses[random.Next(statuses.Length)];
            
            var application = new Application
            {
                CandidateId = candidate.CandidateId,
                RequisitionId = requisition.RequisitionId,
                CurrentStage = stage,
                Status = status,
                // AppliedDate property removed - not in Application entity
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 90)),
                UpdatedAt = DateTime.UtcNow
            };

            applications.Add(application);
            await context.Applications.AddAsync(application);
        }

        await context.SaveChangesAsync();
        return applications;
    }

    private static async Task<List<StageHistory>> CreateCustomStageHistoryAsync(HiringPipelineDbContext context, int count, List<Application> applications)
    {
        var stageHistory = new List<StageHistory>();
        var random = new Random();
        
        var stages = new[] { "Applied", "Phone Screen", "Technical Interview", "Onsite Interview", "Reference Check", "Offer", "Hired", "Rejected" };
        var movedByUsers = new[] { "Admin", "Recruiter", "Hiring Manager", "Interviewer", "System" };
        var reasons = new[] { "Initial application received", "Phone interview completed", "Technical assessment passed", "Onsite interview conducted", "References verified", "Offer extended", "Candidate accepted", "Not a good fit" };

        for (int i = 0; i < count; i++)
        {
            var application = applications[random.Next(applications.Count)];
            var fromStage = random.Next(2) == 0 ? stages[random.Next(stages.Length)] : null;
            var toStage = stages[random.Next(stages.Length)];
            var movedBy = movedByUsers[random.Next(movedByUsers.Length)];
            var reason = reasons[random.Next(reasons.Length)];
            
            var history = new StageHistory
            {
                ApplicationId = application.ApplicationId,
                FromStage = fromStage,
                ToStage = toStage,
                MovedBy = movedBy,
                MovedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                // Reason property removed - not in StageHistory entity
                // Notes property removed - not in StageHistory entity
            };

            stageHistory.Add(history);
            await context.StageHistories.AddAsync(history);
        }

        await context.SaveChangesAsync();
        return stageHistory;
    }

    private static async Task<List<User>> CreateCustomUsersAsync(HiringPipelineDbContext context)
    {
        var users = new List<User>();
        var random = new Random();
        
        var firstNames = new[] { "Alice", "Bob", "Charlie", "Diana", "Eve", "Frank", "Grace", "Henry", "Ivy", "Jack" };
        var lastNames = new[] { "Adams", "Brown", "Clark", "Davis", "Evans", "Foster", "Green", "Hall", "Irwin", "Jones" };
        var roles = new[] { "Recruiter", "Hiring Manager", "Interviewer", "Read-only" };

        for (int i = 0; i < 5; i++) // Create 5 additional users
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var email = $"{firstName.ToLower()}.{lastName.ToLower()}@company.com";
            var role = roles[random.Next(roles.Length)];
            
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                UpdatedAt = DateTime.UtcNow
            };

            users.Add(user);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync(); // Save to get the user ID

            // Assign role
            var roleEntity = await context.Roles.FirstOrDefaultAsync(r => r.Name == role);
            if (roleEntity != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleEntity.Id,
                    AssignedAt = DateTime.UtcNow
                };
                await context.UserRoles.AddAsync(userRole);
            }
        }

        await context.SaveChangesAsync();
        return users;
    }

    private class StageHistoryData
    {
        public string? FromStage { get; set; }
        public string ToStage { get; set; } = string.Empty;
        public string MovedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
