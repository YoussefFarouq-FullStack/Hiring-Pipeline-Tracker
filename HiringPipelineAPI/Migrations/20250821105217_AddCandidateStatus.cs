using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiringPipelineAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Candidates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Candidates");
        }
    }
}
