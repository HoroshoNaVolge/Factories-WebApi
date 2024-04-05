using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Factories.WebApi.BLL.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurationUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe342990-c53a-4bb9-89b6-4b4482e956fb",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "2b5ddc62-ae7e-4ffb-8d1f-9053ebc7ceea", "5a7b4f25-72bf-4559-ac8a-c7501d6ca0a9" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe342990-c53a-4bb9-89b6-4b4482e956fb",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "0bbf3574-511d-46c5-8aba-5d0d2735b11b", "40976921-2918-4629-9054-c572b43c8ebc" });
        }
    }
}
