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
                    Username = "readonly1",
                    Email = "readonly1@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("ReadOnly123!"),
                    FirstName = "David",
                    LastName = "Wilson",
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
                // Admin user gets Admin role
                new UserRole { Id = 1, UserId = 1, RoleId = 1, AssignedAt = DateTime.UtcNow },
                
                // Recruiter user gets Recruiter role
                new UserRole { Id = 2, UserId = 2, RoleId = 2, AssignedAt = DateTime.UtcNow },
                
                // Hiring Manager user gets Hiring Manager role
                new UserRole { Id = 3, UserId = 3, RoleId = 3, AssignedAt = DateTime.UtcNow },
                
                // Interviewer user gets Interviewer role
                new UserRole { Id = 4, UserId = 4, RoleId = 4, AssignedAt = DateTime.UtcNow },
                
                // Read-only user gets Read-only role
                new UserRole { Id = 5, UserId = 5, RoleId = 5, AssignedAt = DateTime.UtcNow }
            };
        }
    }
}
