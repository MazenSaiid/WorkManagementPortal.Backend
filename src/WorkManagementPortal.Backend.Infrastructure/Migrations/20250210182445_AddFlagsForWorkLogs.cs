using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFlagsForWorkLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExceededPauseHours",
                table: "WorkTrackingLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "OvertimeWorkDurationInHours",
                table: "WorkTrackingLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "WorkedOutofSchedule",
                table: "WorkTrackingLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkedOvertime",
                table: "WorkTrackingLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExceededPauseHours",
                table: "WorkTrackingLogs");

            migrationBuilder.DropColumn(
                name: "OvertimeWorkDurationInHours",
                table: "WorkTrackingLogs");

            migrationBuilder.DropColumn(
                name: "WorkedOutofSchedule",
                table: "WorkTrackingLogs");

            migrationBuilder.DropColumn(
                name: "WorkedOvertime",
                table: "WorkTrackingLogs");
        }
    }
}
