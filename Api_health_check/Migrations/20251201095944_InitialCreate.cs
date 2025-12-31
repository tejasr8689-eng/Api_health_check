using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_health_check.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonitoredEndpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckIntervalSeconds = table.Column<int>(type: "int", nullable: false),
                    LastStatusCode = table.Column<int>(type: "int", nullable: true),
                    LastCheckUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDown = table.Column<bool>(type: "bit", nullable: false),
                    ConsecutiveFailures = table.Column<int>(type: "int", nullable: false),
                    NotifyEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoredEndpoints", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitoredEndpoints");
        }
    }
}
