using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_WorkShifts_WorkShiftId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "PauseIsFinished",
                table: "PauseTrackingLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ScreenShotTrackingLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScreenShotTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenShotTrackingLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenShotTrackingLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScreenShotTrackingLogs_UserId",
                table: "ScreenShotTrackingLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_WorkShifts_WorkShiftId",
                table: "AspNetUsers",
                column: "WorkShiftId",
                principalTable: "WorkShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_WorkShifts_WorkShiftId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ScreenShotTrackingLogs");

            migrationBuilder.DropColumn(
                name: "PauseIsFinished",
                table: "PauseTrackingLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_WorkShifts_WorkShiftId",
                table: "AspNetUsers",
                column: "WorkShiftId",
                principalTable: "WorkShifts",
                principalColumn: "Id");
        }
    }
}
