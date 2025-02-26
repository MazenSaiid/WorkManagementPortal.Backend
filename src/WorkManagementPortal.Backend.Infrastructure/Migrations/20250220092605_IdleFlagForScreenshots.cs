using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IdleFlagForScreenshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsIdle",
                table: "ScreenShotTrackingLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIdle",
                table: "ScreenShotTrackingLogs");
        }
    }
}
