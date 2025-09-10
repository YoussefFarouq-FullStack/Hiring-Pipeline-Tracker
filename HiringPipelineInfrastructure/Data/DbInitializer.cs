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

    private class StageHistoryData
    {
        public string? FromStage { get; set; }
        public string ToStage { get; set; } = string.Empty;
        public string MovedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
