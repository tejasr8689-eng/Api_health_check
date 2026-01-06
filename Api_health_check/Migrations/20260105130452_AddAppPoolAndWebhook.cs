using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_health_check.Migrations
{
    /// <inheritdoc />
    public partial class AddAppPoolAndWebhook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppPoolName",
                table: "MonitoredEndpoints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecoveryWebhookUrl",
                table: "MonitoredEndpoints",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppPoolName",
                table: "MonitoredEndpoints");

            migrationBuilder.DropColumn(
                name: "RecoveryWebhookUrl",
                table: "MonitoredEndpoints");
        }
    }
}
