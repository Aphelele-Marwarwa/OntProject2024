using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ont3010_Project_YA2024.Migrations
{
    /// <inheritdoc />
    public partial class Adee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Customers_CustomerId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Employees_EmployeeId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CustomerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_EmployeeId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CustomerId",
                table: "Notifications",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EmployeeId",
                table: "Notifications",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Customers_CustomerId",
                table: "Notifications",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Employees_EmployeeId",
                table: "Notifications",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
