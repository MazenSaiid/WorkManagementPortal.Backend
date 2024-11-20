using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationships : Migration
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

            migrationBuilder.RenameColumn(
                name: "IsFinished",
                table: "WorkTrackingLogs",
                newName: "HasFinished");

            migrationBuilder.RenameColumn(
                name: "ClockOut",
                table: "WorkTrackingLogs",
                newName: "WorkTimeStart");

            migrationBuilder.RenameColumn(
                name: "ClockIn",
                table: "WorkTrackingLogs",
                newName: "WorkTimeEnd");

            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkDate",
                table: "WorkTrackingLogs",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "WorkShifts",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "WorkShifts",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "WorkTrackingLogId",
                table: "PauseTrackingLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkDate",
                table: "PauseTrackingLogs",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkTrackingLogId",
                table: "PauseTrackingLogs",
                column: "WorkTrackingLogId",
                principalTable: "WorkTrackingLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PauseTrackingLogs_WorkTrackingLogs_WorkTrackingLogId",
                table: "PauseTrackingLogs");

            migrationBuilder.DropColumn(
                name: "WorkDate",
                table: "WorkTrackingLogs");

            migrationBuilder.DropColumn(
                name: "WorkDate",
                table: "PauseTrackingLogs");

            migrationBuilder.RenameColumn(
                name: "WorkTimeStart",
                table: "WorkTrackingLogs",
                newName: "ClockOut");

            migrationBuilder.RenameColumn(
                name: "WorkTimeEnd",
                table: "WorkTrackingLogs",
                newName: "ClockIn");

            migrationBuilder.RenameColumn(
                name: "HasFinished",
                table: "WorkTrackingLogs",
                newName: "IsFinished");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "WorkShifts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "WorkShifts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

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
