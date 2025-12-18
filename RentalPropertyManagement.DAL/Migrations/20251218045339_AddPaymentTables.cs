using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalPropertyManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MaintenanceRequests",
                keyColumn: "Id",
                keyValue: 1,
                column: "SubmittedDate",
                value: new DateTime(2025, 12, 18, 11, 53, 39, 251, DateTimeKind.Local).AddTicks(2513));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MaintenanceRequests",
                keyColumn: "Id",
                keyValue: 1,
                column: "SubmittedDate",
                value: new DateTime(2025, 12, 18, 10, 26, 6, 274, DateTimeKind.Local).AddTicks(8573));
        }
    }
}
