using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixWorkShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkShifts_AspNetUsers_UserId",
                table: "WorkShifts");

            migrationBuilder.DropIndex(
                name: "IX_WorkShifts_UserId",
                table: "WorkShifts");

            migrationBuilder.DropColumn(
                name: "TotalPausedHours",
                table: "WorkShifts");

            migrationBuilder.DropColumn(
                name: "TotalWorkedHours",
                table: "WorkShifts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WorkShifts");

            migrationBuilder.AddColumn<double>(
                name: "ActualWorkDuration",
                table: "WorkTrackingLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PauseDuration",
                table: "PauseTrackingLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "WorkShiftId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WorkShiftId",
                table: "AspNetUsers",
                column: "WorkShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_WorkShifts_WorkShiftId",
                table: "AspNetUsers",
                column: "WorkShiftId",
                principalTable: "WorkShifts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_WorkShifts_WorkShiftId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WorkShiftId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ActualWorkDuration",
                table: "WorkTrackingLogs");

            migrationBuilder.DropColumn(
                name: "PauseDuration",
                table: "PauseTrackingLogs");

            migrationBuilder.DropColumn(
                name: "WorkShiftId",
                table: "AspNetUsers");

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
                table: "WorkShifts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WorkShifts_UserId",
                table: "WorkShifts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkShifts_AspNetUsers_UserId",
                table: "WorkShifts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
