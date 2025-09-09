using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiringPipelineInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateResumeAndSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Resume column if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'Resume')
                BEGIN
                    ALTER TABLE [Candidates] ADD [Resume] nvarchar(max) NULL;
                END
            ");

            // Add Skills column if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'Skills')
                BEGIN
                    ALTER TABLE [Candidates] ADD [Skills] nvarchar(max) NULL;
                END
            ");

            // Remove LinkedInUrl column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'LinkedInUrl')
                BEGIN
                    ALTER TABLE [Candidates] DROP COLUMN [LinkedInUrl];
                END
            ");

            // Remove Source column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'Source')
                BEGIN
                    ALTER TABLE [Candidates] DROP COLUMN [Source];
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Resume column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'Resume')
                BEGIN
                    ALTER TABLE [Candidates] DROP COLUMN [Resume];
                END
            ");

            // Remove Skills column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'Skills')
                BEGIN
                    ALTER TABLE [Candidates] DROP COLUMN [Skills];
                END
            ");

            // Add back LinkedInUrl column
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'LinkedInUrl')
                BEGIN
                    ALTER TABLE [Candidates] ADD [LinkedInUrl] nvarchar(max) NULL;
                END
            ");

            // Add back Source column
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'Source')
                BEGIN
                    ALTER TABLE [Candidates] ADD [Source] nvarchar(max) NULL;
                END
            ");
        }
    }
}
