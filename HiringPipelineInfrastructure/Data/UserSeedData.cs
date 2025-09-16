using HiringPipelineCore.Entities;
using BCrypt.Net;

namespace HiringPipelineInfrastructure.Data
{
    public static class UserSeedData
    {
        public static List<User> GetDefaultUsers()
        {
            return new List<User>
            {
                // Admin Users
                new User
                {
                    Username = "admin",
                    Email = "admin@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    FirstName = "System",
                    LastName = "Administrator",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "admin2",
                    Email = "admin2@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    FirstName = "Jennifer",
                    LastName = "Smith",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Recruiter Users
                new User
                {
                    Username = "recruiter1",
                    Email = "recruiter1@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Recruiter123!"),
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "recruiter2",
                    Email = "recruiter2@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Recruiter123!"),
                    FirstName = "James",
                    LastName = "Miller",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "recruiter3",
                    Email = "recruiter3@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Recruiter123!"),
                    FirstName = "Lisa",
                    LastName = "Garcia",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Hiring Manager Users
                new User
                {
                    Username = "hiringmanager1",
                    Email = "hiringmanager1@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                    FirstName = "Michael",
                    LastName = "Brown",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "hiringmanager2",
                    Email = "hiringmanager2@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                    FirstName = "Amanda",
                    LastName = "Taylor",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "hiringmanager3",
                    Email = "hiringmanager3@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                    FirstName = "Robert",
                    LastName = "Anderson",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Interviewer Users
                new User
                {
                    Username = "interviewer1",
                    Email = "interviewer1@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Interviewer123!"),
                    FirstName = "Emily",
                    LastName = "Davis",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "interviewer2",
                    Email = "interviewer2@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Interviewer123!"),
                    FirstName = "Christopher",
                    LastName = "Martinez",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "interviewer3",
                    Email = "interviewer3@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Interviewer123!"),
                    FirstName = "Jessica",
                    LastName = "Rodriguez",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "interviewer4",
                    Email = "interviewer4@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Interviewer123!"),
                    FirstName = "Daniel",
                    LastName = "Lee",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Read-only Users
                new User
                {
                    Username = "readonly1",
                    Email = "readonly1@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("ReadOnly123!"),
                    FirstName = "David",
                    LastName = "Wilson",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "readonly2",
                    Email = "readonly2@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("ReadOnly123!"),
                    FirstName = "Maria",
                    LastName = "Gonzalez",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "readonly3",
                    Email = "readonly3@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("ReadOnly123!"),
                    FirstName = "Kevin",
                    LastName = "Thompson",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }

        public static List<UserRole> GetDefaultUserRoles()
        {
            return new List<UserRole>
            {
                // Admin Users (Role ID 1)
                new UserRole { Id = 1, UserId = 1, RoleId = 1, AssignedAt = DateTime.UtcNow }, // admin
                new UserRole { Id = 2, UserId = 2, RoleId = 1, AssignedAt = DateTime.UtcNow }, // admin2
                
                // Recruiter Users (Role ID 2)
                new UserRole { Id = 3, UserId = 3, RoleId = 2, AssignedAt = DateTime.UtcNow }, // recruiter1
                new UserRole { Id = 4, UserId = 4, RoleId = 2, AssignedAt = DateTime.UtcNow }, // recruiter2
                new UserRole { Id = 5, UserId = 5, RoleId = 2, AssignedAt = DateTime.UtcNow }, // recruiter3
                
                // Hiring Manager Users (Role ID 3)
                new UserRole { Id = 6, UserId = 6, RoleId = 3, AssignedAt = DateTime.UtcNow }, // hiringmanager1
                new UserRole { Id = 7, UserId = 7, RoleId = 3, AssignedAt = DateTime.UtcNow }, // hiringmanager2
                new UserRole { Id = 8, UserId = 8, RoleId = 3, AssignedAt = DateTime.UtcNow }, // hiringmanager3
                
                // Interviewer Users (Role ID 4)
                new UserRole { Id = 9, UserId = 9, RoleId = 4, AssignedAt = DateTime.UtcNow }, // interviewer1
                new UserRole { Id = 10, UserId = 10, RoleId = 4, AssignedAt = DateTime.UtcNow }, // interviewer2
                new UserRole { Id = 11, UserId = 11, RoleId = 4, AssignedAt = DateTime.UtcNow }, // interviewer3
                new UserRole { Id = 12, UserId = 12, RoleId = 4, AssignedAt = DateTime.UtcNow }, // interviewer4
                
                // Read-only Users (Role ID 5)
                new UserRole { Id = 13, UserId = 13, RoleId = 5, AssignedAt = DateTime.UtcNow }, // readonly1
                new UserRole { Id = 14, UserId = 14, RoleId = 5, AssignedAt = DateTime.UtcNow }, // readonly2
                new UserRole { Id = 15, UserId = 15, RoleId = 5, AssignedAt = DateTime.UtcNow } // readonly3
            };
        }
    }
}
