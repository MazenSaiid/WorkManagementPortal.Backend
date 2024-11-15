using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkManagementPortal.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSupervisor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsTeamLeader",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSupervisor",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTeamLeader",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
