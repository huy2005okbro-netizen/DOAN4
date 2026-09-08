using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MassageShop.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Administrator", "$2a$11$xMOY5sDtKfqRqUYeBTDuKOvMpiQ5R8RR.R/iSFpyWsGi1gkrRjlHi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Admin", "$2a$11$do7xQWga3zqoK2GU6sPLz.tmjH/Pg92m.isYbTIHG2sWAsvNUqL/y" });
        }
    }
}
