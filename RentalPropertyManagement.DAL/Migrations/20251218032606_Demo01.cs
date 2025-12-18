using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RentalPropertyManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Demo01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Contracts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Address", "City", "Description", "IsOccupied", "MonthlyRent", "SquareFootage" },
                values: new object[,]
                {
                    { 1, "123 Main St", "Hanoi", "Apartment in city center", false, 10000000m, 100 },
                    { 2, "456 Elm St", "Saigon", "House with garden", true, 15000000m, 150 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PasswordHash", "PhoneNumber", "Role" },
                values: new object[,]
                {
                    { 1, "landlord@gmail.com", "Landlord", "Admin", "$2y$10$tlwzkAKGJOgk8yPycei6O.ZK4XloYY2mj.G91apzS7sJyn1x3vXn2", "0123456789", 1 },
                    { 2, "tenant@gmail.com", "Tenant", "User", "$2y$10$tlwzkAKGJOgk8yPycei6O.ZK4XloYY2mj.G91apzS7sJyn1x3vXn2", "0987654321", 2 },
                    { 3, "provider@gmail.com", "Service", "Provider", "$2y$10$tlwzkAKGJOgk8yPycei6O.ZK4XloYY2mj.G91apzS7sJyn1x3vXn2", "0112233445", 3 }
                });

            migrationBuilder.InsertData(
                table: "Contracts",
                columns: new[] { "Id", "EndDate", "PropertyId", "RentAmount", "StartDate", "Status", "TenantId" },
                values: new object[] { 1, new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10000000m, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2 });

            migrationBuilder.InsertData(
                table: "MaintenanceRequests",
                columns: new[] { "Id", "AssignedProviderId", "AttachmentUrl", "Description", "Priority", "PropertyId", "Status", "SubmittedDate", "TenantId" },
                values: new object[] { 1, null, "example.jpg", "Fix leaking roof", 3, 1, 1, new DateTime(2025, 12, 18, 10, 26, 6, 274, DateTimeKind.Local).AddTicks(8573), 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MaintenanceRequests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
