using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNamings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasCheckedOutAEarly",
                table: "WorkTrackingLogs",
                newName: "HasCheckedOutEarly");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasCheckedOutEarly",
                table: "WorkTrackingLogs",
                newName: "HasCheckedOutAEarly");
        }
    }
}
