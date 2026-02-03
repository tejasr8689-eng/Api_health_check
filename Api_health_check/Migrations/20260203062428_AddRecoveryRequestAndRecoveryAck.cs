using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_health_check.Migrations
{
    /// <inheritdoc />
    public partial class AddRecoveryRequestAndRecoveryAck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecoveryRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndpointId = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoveryRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecoveryAcks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecoveryRequestId = table.Column<int>(type: "int", nullable: false),
                    requestId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    completedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    receivedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoveryAcks", x => x.id);
                    table.ForeignKey(
                        name: "FK_RecoveryAcks_RecoveryRequests_RecoveryRequestId",
                        column: x => x.RecoveryRequestId,
                        principalTable: "RecoveryRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecoveryAcks_RecoveryRequestId",
                table: "RecoveryAcks",
                column: "RecoveryRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecoveryAcks");

            migrationBuilder.DropTable(
                name: "RecoveryRequests");
        }
    }
}
