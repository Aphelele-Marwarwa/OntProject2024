using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ont3010_Project_YA2024.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeNotificationStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeNotificationStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeNotificationStatuses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_EmployeeNotificationStatuses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_EmployeeNotificationStatuses_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeNotificationStatuses_CustomerId",
                table: "EmployeeNotificationStatuses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeNotificationStatuses_EmployeeId",
                table: "EmployeeNotificationStatuses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeNotificationStatuses_NotificationId",
                table: "EmployeeNotificationStatuses",
                column: "NotificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeNotificationStatuses");
        }
    }
}
