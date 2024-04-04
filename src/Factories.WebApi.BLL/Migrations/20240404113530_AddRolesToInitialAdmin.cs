using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Factories.WebApi.BLL.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToInitialAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "66f7058e-10a9-42f7-8bf1-62e5e117e49c", null, "User", "USER" },
                    { "70a2bf46-7e0a-4559-a625-5b3ca954b3cf", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "66f7058e-10a9-42f7-8bf1-62e5e117e49c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "70a2bf46-7e0a-4559-a625-5b3ca954b3cf");
        }
    }
}
