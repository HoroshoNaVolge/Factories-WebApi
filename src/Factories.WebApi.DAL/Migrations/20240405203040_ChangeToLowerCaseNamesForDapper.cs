using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Factories.WebApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToLowerCaseNamesForDapper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tanks_Units_UnitId",
                table: "Tanks");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_Factories_FactoryId",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Units",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tanks",
                table: "Tanks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Factories",
                table: "Factories");

            migrationBuilder.RenameTable(
                name: "Units",
                newName: "units");

            migrationBuilder.RenameTable(
                name: "Tanks",
                newName: "tanks");

            migrationBuilder.RenameTable(
                name: "Factories",
                newName: "factories");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "units",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "FactoryId",
                table: "units",
                newName: "factoryid");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "units",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "units",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Units_FactoryId",
                table: "units",
                newName: "IX_units_factoryid");

            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "tanks",
                newName: "volume");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "tanks",
                newName: "unitid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "tanks",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "MaxVolume",
                table: "tanks",
                newName: "maxvolume");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "tanks",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tanks",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Tanks_UnitId",
                table: "tanks",
                newName: "IX_tanks_unitid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "factories",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "factories",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "factories",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_units",
                table: "units",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tanks",
                table: "tanks",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_factories",
                table: "factories",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_tanks_units_unitid",
                table: "tanks",
                column: "unitid",
                principalTable: "units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_units_factories_factoryid",
                table: "units",
                column: "factoryid",
                principalTable: "factories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tanks_units_unitid",
                table: "tanks");

            migrationBuilder.DropForeignKey(
                name: "FK_units_factories_factoryid",
                table: "units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_units",
                table: "units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tanks",
                table: "tanks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_factories",
                table: "factories");

            migrationBuilder.RenameTable(
                name: "units",
                newName: "Units");

            migrationBuilder.RenameTable(
                name: "tanks",
                newName: "Tanks");

            migrationBuilder.RenameTable(
                name: "factories",
                newName: "Factories");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Units",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "factoryid",
                table: "Units",
                newName: "FactoryId");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Units",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Units",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_units_factoryid",
                table: "Units",
                newName: "IX_Units_FactoryId");

            migrationBuilder.RenameColumn(
                name: "volume",
                table: "Tanks",
                newName: "Volume");

            migrationBuilder.RenameColumn(
                name: "unitid",
                table: "Tanks",
                newName: "UnitId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Tanks",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "maxvolume",
                table: "Tanks",
                newName: "MaxVolume");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Tanks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Tanks",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_tanks_unitid",
                table: "Tanks",
                newName: "IX_Tanks_UnitId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Factories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Factories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Factories",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Units",
                table: "Units",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tanks",
                table: "Tanks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Factories",
                table: "Factories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tanks_Units_UnitId",
                table: "Tanks",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Factories_FactoryId",
                table: "Units",
                column: "FactoryId",
                principalTable: "Factories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
