using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkLogIdInScreenShots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkTrackingLogId",
                table: "ScreenShotTrackingLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScreenShotTrackingLogs_WorkTrackingLogId",
                table: "ScreenShotTrackingLogs",
                column: "WorkTrackingLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScreenShotTrackingLogs_WorkTrackingLogs_WorkTrackingLogId",
                table: "ScreenShotTrackingLogs",
                column: "WorkTrackingLogId",
                principalTable: "WorkTrackingLogs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScreenShotTrackingLogs_WorkTrackingLogs_WorkTrackingLogId",
                table: "ScreenShotTrackingLogs");

            migrationBuilder.DropIndex(
                name: "IX_ScreenShotTrackingLogs_WorkTrackingLogId",
                table: "ScreenShotTrackingLogs");

            migrationBuilder.DropColumn(
                name: "WorkTrackingLogId",
                table: "ScreenShotTrackingLogs");
        }
    }
}
