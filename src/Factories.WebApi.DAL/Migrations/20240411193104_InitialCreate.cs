using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Factories.WebApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "factories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_factories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    factoryid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units", x => x.id);
                    table.ForeignKey(
                        name: "FK_units_factories_factoryid",
                        column: x => x.factoryid,
                        principalTable: "factories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tanks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    unitid = table.Column<int>(type: "integer", nullable: false),
                    volume = table.Column<int>(type: "integer", nullable: false),
                    maxvolume = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tanks", x => x.id);
                    table.ForeignKey(
                        name: "FK_tanks_units_unitid",
                        column: x => x.unitid,
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "factories",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Первый нефтеперерабатывающий завод", "НПЗ№1" },
                    { 2, "Второй нефтеперерабатывающий завод", "НПЗ№2" }
                });

            migrationBuilder.InsertData(
                table: "units",
                columns: new[] { "id", "description", "factoryid", "name" },
                values: new object[,]
                {
                    { 1, "Газофракционирующая установка", 1, "ГФУ-2" },
                    { 2, "Атмосферно-вакуумная трубчатка", 1, "АВТ-6" },
                    { 3, "Атмосферно - вакуумная трубчатка", 2, "АВТ-10" }
                });

            migrationBuilder.InsertData(
                table: "tanks",
                columns: new[] { "id", "description", "maxvolume", "name", "unitid", "volume" },
                values: new object[,]
                {
                    { 1, "Надземный-вертикальный", 2000, "Резервуар 1", 1, 1500 },
                    { 2, "Надземный-горизонтальный", 3000, "Резервуар 2", 1, 2500 },
                    { 3, "Надземный-горизонтальный", 3000, "Резервуар 3", 2, 3000 },
                    { 4, "Надземный-вертикальный", 3000, "Резервуар 4", 2, 3000 },
                    { 5, "Подземный-двустенный", 5000, "Резервуар 5", 2, 4000 },
                    { 6, "Подводный", 500, "Резервуар 6", 2, 500 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tanks_unitid",
                table: "tanks",
                column: "unitid");

            migrationBuilder.CreateIndex(
                name: "IX_units_factoryid",
                table: "units",
                column: "factoryid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tanks");

            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropTable(
                name: "factories");
        }
    }
}
