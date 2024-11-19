using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkLogId",
                table: "PauseTrackingLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkTrackingLogId",
                table: "PauseTrackingLogs");

            migrationBuilder.DropIndex(
                name: "IX_PauseTrackingLogs_WorkLogId",
                table: "PauseTrackingLogs");

            migrationBuilder.DropColumn(
                name: "WorkLogId",
                table: "PauseTrackingLogs");

            migrationBuilder.AlterColumn<int>(
                name: "WorkTrackingLogId",
                table: "PauseTrackingLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkTrackingLogId1",
                table: "PauseTrackingLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PauseTrackingLogs_WorkTrackingLogId1",
                table: "PauseTrackingLogs",
                column: "WorkTrackingLogId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkTrackingLogId",
                table: "PauseTrackingLogs",
                column: "WorkTrackingLogId",
                principalTable: "WorkTrackingLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkTrackingLogId1",
                table: "PauseTrackingLogs",
                column: "WorkTrackingLogId1",
                principalTable: "WorkTrackingLogs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkTrackingLogId",
                table: "PauseTrackingLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkTrackingLogId1",
                table: "PauseTrackingLogs");

            migrationBuilder.DropIndex(
                name: "IX_PauseTrackingLogs_WorkTrackingLogId1",
                table: "PauseTrackingLogs");

            migrationBuilder.DropColumn(
                name: "WorkTrackingLogId1",
                table: "PauseTrackingLogs");

            migrationBuilder.AlterColumn<int>(
                name: "WorkTrackingLogId",
                table: "PauseTrackingLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "WorkLogId",
                table: "PauseTrackingLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PauseTrackingLogs_WorkLogId",
                table: "PauseTrackingLogs",
                column: "WorkLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkLogId",
                table: "PauseTrackingLogs",
                column: "WorkLogId",
                principalTable: "WorkTrackingLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkTrackingLogId",
                table: "PauseTrackingLogs",
                column: "WorkTrackingLogId",
                principalTable: "WorkTrackingLogs",
                principalColumn: "Id");
        }
    }
}
