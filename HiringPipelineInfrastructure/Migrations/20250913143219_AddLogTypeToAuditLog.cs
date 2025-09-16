using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiringPipelineInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLogTypeToAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LogType",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogType",
                table: "AuditLogs");
        }
    }
}
