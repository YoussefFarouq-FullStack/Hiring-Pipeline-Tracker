using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiringPipelineInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityAndDraftFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Applications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHighPriority",
                table: "Applications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "IsHighPriority",
                table: "Applications");
        }
    }
}
