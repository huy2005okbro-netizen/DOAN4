using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MassageShop.API.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Ydmgv.TKiTeIFj0M2M7slethYseqAA5wKUk08.qFdUwptqoyErqDy");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Phone", "RoleId" },
                values: new object[,]
                {
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nhanvien1@massageshop.com", "Nguyễn Thị Lan", true, "$2a$11$wEfXF6uEp/zQzxQDxUXuj..nvAgfhJYL.DHYsqPjBOMb/y7Z2WhC.", "0911111111", 2 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nhanvien2@massageshop.com", "Trần Văn Minh", true, "$2a$11$wEfXF6uEp/zQzxQDxUXuj..nvAgfhJYL.DHYsqPjBOMb/y7Z2WhC.", "0922222222", 2 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "khachhang1@gmail.com", "Lê Thị Hoa", true, "$2a$11$cRKxiBLbNU/BaQ3j57WIEeM/GCFsBxMCYxfN.4SyMPvXsMoG05OXK", "0933333333", 3 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "CreatedAt", "DateOfBirth", "Gender", "LoyaltyPoints", "UserId" },
                values: new object[] { 1, "123 Đường Lê Lợi, Q.1, TP.HCM", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1995, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Nữ", 100, 4 });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "IsActive", "Position", "StartDate", "UserId" },
                values: new object[,]
                {
                    { 1, true, "Kỹ thuật viên massage", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 2, true, "Kỹ thuật viên chăm sóc da", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$xMOY5sDtKfqRqUYeBTDuKOvMpiQ5R8RR.R/iSFpyWsGi1gkrRjlHi");
        }
    }
}
