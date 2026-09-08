using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MassageShop.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$4n/2FyQASRibDV8kNuQXwePwHTJD6r/oko6vIjXNB0cFETaWssqPK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$do7xQWga3zqoK2GU6sPLz.tmjH/Pg92m.isYbTIHG2sWAsvNUqL/y");
        }
    }
}
