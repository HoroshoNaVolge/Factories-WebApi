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
                name: "Factories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FactoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Factories_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "Factories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UnitId = table.Column<int>(type: "integer", nullable: false),
                    Volume = table.Column<double>(type: "double precision", nullable: true),
                    MaxVolume = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tanks_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Factories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Первый нефтеперерабатывающий завод", "НПЗ№1" },
                    { 2, "Второй нефтеперерабатывающий завод", "НПЗ№2" }
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "Description", "FactoryId", "Name" },
                values: new object[,]
                {
                    { 1, "Газофракционирующая установка", 1, "ГФУ-2" },
                    { 2, "Атмосферно-вакуумная трубчатка", 1, "АВТ-6" },
                    { 3, "Атмосферно - вакуумная трубчатка", 2, "АВТ-10" }
                });

            migrationBuilder.InsertData(
                table: "Tanks",
                columns: new[] { "Id", "Description", "MaxVolume", "Name", "UnitId", "Volume" },
                values: new object[,]
                {
                    { 1, "Надземный-вертикальный", 2000.0, "Резервуар 1", 1, 1500.0 },
                    { 2, "Надземный-горизонтальный", 3000.0, "Резервуар 2", 1, 2500.0 },
                    { 3, "Надземный-горизонтальный", 3000.0, "Резервуар 3", 2, 3000.0 },
                    { 4, "Надземный-вертикальный", 3000.0, "Резервуар 4", 2, 3000.0 },
                    { 5, "Подземный-двустенный", 5000.0, "Резервуар 5", 2, 4000.0 },
                    { 6, "Подводный", 500.0, "Резервуар 6", 2, 500.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tanks_UnitId",
                table: "Tanks",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_FactoryId",
                table: "Units",
                column: "FactoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tanks");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Factories");
        }
    }
}
