using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOutBoxMessageClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClaimId",
                table: "OutboxMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClaimedUntil",
                table: "OutboxMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ClaimedUntil_ProcessedAt",
                table: "OutboxMessages",
                columns: new[] { "ClaimedUntil", "ProcessedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_ClaimedUntil_ProcessedAt",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "ClaimId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "ClaimedUntil",
                table: "OutboxMessages");
        }
    }
}
