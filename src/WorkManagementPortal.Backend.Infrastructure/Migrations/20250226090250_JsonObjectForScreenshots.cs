using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class JsonObjectForScreenshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerializedTrackingObject",
                table: "ScreenShotTrackingLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerializedTrackingObject",
                table: "ScreenShotTrackingLogs");
        }
    }
}
