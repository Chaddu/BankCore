using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerIdToIdempotencyRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IdempotencyRecords_Key",
                table: "IdempotencyRecords");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "IdempotencyRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRecords_CustomerId_Key",
                table: "IdempotencyRecords",
                columns: new[] { "CustomerId", "Key" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IdempotencyRecords_Customers_CustomerId",
                table: "IdempotencyRecords",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdempotencyRecords_Customers_CustomerId",
                table: "IdempotencyRecords");

            migrationBuilder.DropIndex(
                name: "IX_IdempotencyRecords_CustomerId_Key",
                table: "IdempotencyRecords");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "IdempotencyRecords");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRecords_Key",
                table: "IdempotencyRecords",
                column: "Key",
                unique: true);
        }
    }
}
