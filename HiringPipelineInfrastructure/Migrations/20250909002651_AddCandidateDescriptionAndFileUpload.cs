using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiringPipelineInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateDescriptionAndFileUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Resume",
                table: "Candidates",
                newName: "ResumeFilePath");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumeFileName",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "Candidates");

            migrationBuilder.RenameColumn(
                name: "ResumeFilePath",
                table: "Candidates",
                newName: "Resume");
        }
    }
}
