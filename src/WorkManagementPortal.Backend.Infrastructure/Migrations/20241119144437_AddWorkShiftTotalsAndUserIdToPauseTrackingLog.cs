using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkShiftTotalsAndUserIdToPauseTrackingLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalPausedHours",
                table: "WorkShifts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWorkedHours",
                table: "WorkShifts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PauseTrackingLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPausedHours",
                table: "WorkShifts");

            migrationBuilder.DropColumn(
                name: "TotalWorkedHours",
                table: "WorkShifts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PauseTrackingLogs");
        }
    }
}
