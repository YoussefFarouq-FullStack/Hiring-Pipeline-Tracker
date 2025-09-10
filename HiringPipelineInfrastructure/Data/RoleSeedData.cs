using HiringPipelineCore.Entities;

namespace HiringPipelineInfrastructure.Data
{
    public static class RoleSeedData
    {
        public static List<Role> GetDefaultRoles()
        {
            return new List<Role>
            {
                new Role
                {
                    Name = "Admin",
                    Description = "Full control: manage users, roles, requisitions, candidates, applications",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Name = "Recruiter",
                    Description = "Can create/manage requisitions, add candidates, move them through stages",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Name = "Hiring Manager",
                    Description = "Reviews candidates, gives feedback, moves to next stages",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Name = "Interviewer",
                    Description = "Limited access: view assigned candidates, submit interview feedback",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Name = "Read-only",
                    Description = "Can only view requisitions or candidate profiles, no edits",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }

        public static List<Permission> GetDefaultPermissions()
        {
            return new List<Permission>
            {
                // System Permissions
                new Permission { Name = "ManageUsers", Resource = "System", Action = "Manage", Description = "Manage users and their roles" },
                new Permission { Name = "ManageRoles", Resource = "System", Action = "Manage", Description = "Manage roles and permissions" },
                new Permission { Name = "ViewAnalytics", Resource = "System", Action = "View", Description = "View system analytics and reports" },
                new Permission { Name = "ConfigurePipeline", Resource = "System", Action = "Configure", Description = "Configure pipeline stages" },

                // Requisition Permissions
                new Permission { Name = "CreateRequisition", Resource = "Requisition", Action = "Create", Description = "Create new requisitions" },
                new Permission { Name = "EditRequisition", Resource = "Requisition", Action = "Edit", Description = "Edit existing requisitions" },
                new Permission { Name = "DeleteRequisition", Resource = "Requisition", Action = "Delete", Description = "Delete requisitions" },
                new Permission { Name = "ViewRequisition", Resource = "Requisition", Action = "View", Description = "View requisitions" },
                new Permission { Name = "AssignRequisition", Resource = "Requisition", Action = "Assign", Description = "Assign requisitions to users" },
                new Permission { Name = "ArchiveRequisition", Resource = "Requisition", Action = "Archive", Description = "Archive requisitions" },

                // Candidate Permissions
                new Permission { Name = "CreateCandidate", Resource = "Candidate", Action = "Create", Description = "Add new candidates" },
                new Permission { Name = "EditCandidate", Resource = "Candidate", Action = "Edit", Description = "Edit candidate information" },
                new Permission { Name = "DeleteCandidate", Resource = "Candidate", Action = "Delete", Description = "Delete candidates" },
                new Permission { Name = "ViewCandidate", Resource = "Candidate", Action = "View", Description = "View candidate profiles" },
                new Permission { Name = "AssignCandidate", Resource = "Candidate", Action = "Assign", Description = "Assign candidates to requisitions" },

                // Application Permissions
                new Permission { Name = "MoveStage", Resource = "Application", Action = "Move", Description = "Move applications through stages" },
                new Permission { Name = "RejectApplication", Resource = "Application", Action = "Reject", Description = "Reject applications" },
                new Permission { Name = "HireCandidate", Resource = "Application", Action = "Hire", Description = "Hire candidates" },
                new Permission { Name = "ViewApplication", Resource = "Application", Action = "View", Description = "View applications" },
                new Permission { Name = "AddFeedback", Resource = "Application", Action = "Feedback", Description = "Add feedback to applications" },

                // Stage History Permissions
                new Permission { Name = "ViewStageHistory", Resource = "StageHistory", Action = "View", Description = "View stage history" },
                new Permission { Name = "CreateStageHistory", Resource = "StageHistory", Action = "Create", Description = "Create stage history entries" }
            };
        }

        public static List<RolePermission> GetDefaultRolePermissions()
        {
            return new List<RolePermission>
            {
                // Admin - All permissions
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1, AssignedAt = DateTime.UtcNow }, // ManageUsers
                new RolePermission { Id = 2, RoleId = 1, PermissionId = 2, AssignedAt = DateTime.UtcNow }, // ManageRoles
                new RolePermission { Id = 3, RoleId = 1, PermissionId = 3, AssignedAt = DateTime.UtcNow }, // ViewAnalytics
                new RolePermission { Id = 4, RoleId = 1, PermissionId = 4, AssignedAt = DateTime.UtcNow }, // ConfigurePipeline
                new RolePermission { Id = 5, RoleId = 1, PermissionId = 5, AssignedAt = DateTime.UtcNow }, // CreateRequisition
                new RolePermission { Id = 6, RoleId = 1, PermissionId = 6, AssignedAt = DateTime.UtcNow }, // EditRequisition
                new RolePermission { Id = 7, RoleId = 1, PermissionId = 7, AssignedAt = DateTime.UtcNow }, // DeleteRequisition
                new RolePermission { Id = 8, RoleId = 1, PermissionId = 8, AssignedAt = DateTime.UtcNow }, // ViewRequisition
                new RolePermission { Id = 9, RoleId = 1, PermissionId = 9, AssignedAt = DateTime.UtcNow }, // AssignRequisition
                new RolePermission { Id = 10, RoleId = 1, PermissionId = 10, AssignedAt = DateTime.UtcNow }, // ArchiveRequisition
                new RolePermission { Id = 11, RoleId = 1, PermissionId = 11, AssignedAt = DateTime.UtcNow }, // CreateCandidate
                new RolePermission { Id = 12, RoleId = 1, PermissionId = 12, AssignedAt = DateTime.UtcNow }, // EditCandidate
                new RolePermission { Id = 13, RoleId = 1, PermissionId = 13, AssignedAt = DateTime.UtcNow }, // DeleteCandidate
                new RolePermission { Id = 14, RoleId = 1, PermissionId = 14, AssignedAt = DateTime.UtcNow }, // ViewCandidate
                new RolePermission { Id = 15, RoleId = 1, PermissionId = 15, AssignedAt = DateTime.UtcNow }, // AssignCandidate
                new RolePermission { Id = 16, RoleId = 1, PermissionId = 16, AssignedAt = DateTime.UtcNow }, // MoveStage
                new RolePermission { Id = 17, RoleId = 1, PermissionId = 17, AssignedAt = DateTime.UtcNow }, // RejectApplication
                new RolePermission { Id = 18, RoleId = 1, PermissionId = 18, AssignedAt = DateTime.UtcNow }, // HireCandidate
                new RolePermission { Id = 19, RoleId = 1, PermissionId = 19, AssignedAt = DateTime.UtcNow }, // ViewApplication
                new RolePermission { Id = 20, RoleId = 1, PermissionId = 20, AssignedAt = DateTime.UtcNow }, // AddFeedback
                new RolePermission { Id = 21, RoleId = 1, PermissionId = 21, AssignedAt = DateTime.UtcNow }, // ViewStageHistory
                new RolePermission { Id = 22, RoleId = 1, PermissionId = 22, AssignedAt = DateTime.UtcNow }, // CreateStageHistory

                // Recruiter - Requisition and Candidate management
                new RolePermission { Id = 23, RoleId = 2, PermissionId = 5, AssignedAt = DateTime.UtcNow }, // CreateRequisition
                new RolePermission { Id = 24, RoleId = 2, PermissionId = 6, AssignedAt = DateTime.UtcNow }, // EditRequisition
                new RolePermission { Id = 25, RoleId = 2, PermissionId = 8, AssignedAt = DateTime.UtcNow }, // ViewRequisition
                new RolePermission { Id = 26, RoleId = 2, PermissionId = 9, AssignedAt = DateTime.UtcNow }, // AssignRequisition
                new RolePermission { Id = 27, RoleId = 2, PermissionId = 10, AssignedAt = DateTime.UtcNow }, // ArchiveRequisition
                new RolePermission { Id = 28, RoleId = 2, PermissionId = 11, AssignedAt = DateTime.UtcNow }, // CreateCandidate
                new RolePermission { Id = 29, RoleId = 2, PermissionId = 12, AssignedAt = DateTime.UtcNow }, // EditCandidate
                new RolePermission { Id = 30, RoleId = 2, PermissionId = 14, AssignedAt = DateTime.UtcNow }, // ViewCandidate
                new RolePermission { Id = 31, RoleId = 2, PermissionId = 15, AssignedAt = DateTime.UtcNow }, // AssignCandidate
                new RolePermission { Id = 32, RoleId = 2, PermissionId = 16, AssignedAt = DateTime.UtcNow }, // MoveStage
                new RolePermission { Id = 33, RoleId = 2, PermissionId = 19, AssignedAt = DateTime.UtcNow }, // ViewApplication
                new RolePermission { Id = 34, RoleId = 2, PermissionId = 21, AssignedAt = DateTime.UtcNow }, // ViewStageHistory
                new RolePermission { Id = 35, RoleId = 2, PermissionId = 22, AssignedAt = DateTime.UtcNow }, // CreateStageHistory

                // Hiring Manager - Review and feedback
                new RolePermission { Id = 36, RoleId = 3, PermissionId = 8, AssignedAt = DateTime.UtcNow }, // ViewRequisition
                new RolePermission { Id = 37, RoleId = 3, PermissionId = 14, AssignedAt = DateTime.UtcNow }, // ViewCandidate
                new RolePermission { Id = 38, RoleId = 3, PermissionId = 16, AssignedAt = DateTime.UtcNow }, // MoveStage
                new RolePermission { Id = 39, RoleId = 3, PermissionId = 17, AssignedAt = DateTime.UtcNow }, // RejectApplication
                new RolePermission { Id = 40, RoleId = 3, PermissionId = 18, AssignedAt = DateTime.UtcNow }, // HireCandidate
                new RolePermission { Id = 41, RoleId = 3, PermissionId = 19, AssignedAt = DateTime.UtcNow }, // ViewApplication
                new RolePermission { Id = 42, RoleId = 3, PermissionId = 20, AssignedAt = DateTime.UtcNow }, // AddFeedback
                new RolePermission { Id = 43, RoleId = 3, PermissionId = 21, AssignedAt = DateTime.UtcNow }, // ViewStageHistory

                // Interviewer - Limited access
                new RolePermission { Id = 44, RoleId = 4, PermissionId = 14, AssignedAt = DateTime.UtcNow }, // ViewCandidate
                new RolePermission { Id = 45, RoleId = 4, PermissionId = 19, AssignedAt = DateTime.UtcNow }, // ViewApplication
                new RolePermission { Id = 46, RoleId = 4, PermissionId = 20, AssignedAt = DateTime.UtcNow }, // AddFeedback
                new RolePermission { Id = 47, RoleId = 4, PermissionId = 21, AssignedAt = DateTime.UtcNow }, // ViewStageHistory

                // Read-only - View only
                new RolePermission { Id = 48, RoleId = 5, PermissionId = 8, AssignedAt = DateTime.UtcNow }, // ViewRequisition
                new RolePermission { Id = 49, RoleId = 5, PermissionId = 14, AssignedAt = DateTime.UtcNow }, // ViewCandidate
                new RolePermission { Id = 50, RoleId = 5, PermissionId = 19, AssignedAt = DateTime.UtcNow }, // ViewApplication
                new RolePermission { Id = 51, RoleId = 5, PermissionId = 21, AssignedAt = DateTime.UtcNow } // ViewStageHistory
            };
        }
    }
}
