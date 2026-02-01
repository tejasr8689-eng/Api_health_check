using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_health_check.Migrations
{
    /// <inheritdoc />
    public partial class AddRecoveryCooldown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecoveryWebhookUrl",
                table: "RecoveryAuditLos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRecoveryUtc",
                table: "MonitoredEndpoints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecoveryCoolDownSeconds",
                table: "MonitoredEndpoints",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecoveryWebhookUrl",
                table: "RecoveryAuditLos");

            migrationBuilder.DropColumn(
                name: "LastRecoveryUtc",
                table: "MonitoredEndpoints");

            migrationBuilder.DropColumn(
                name: "RecoveryCoolDownSeconds",
                table: "MonitoredEndpoints");
        }
    }
}
