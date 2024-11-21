using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActualWorkDuration",
                table: "WorkTrackingLogs",
                newName: "ActualWorkDurationInHours");

            migrationBuilder.RenameColumn(
                name: "PauseDuration",
                table: "PauseTrackingLogs",
                newName: "PauseDurationInMinutes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActualWorkDurationInHours",
                table: "WorkTrackingLogs",
                newName: "ActualWorkDuration");

            migrationBuilder.RenameColumn(
                name: "PauseDurationInMinutes",
                table: "PauseTrackingLogs",
                newName: "PauseDuration");
        }
    }
}
