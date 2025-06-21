using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class FixRecipeIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Ingredients_Unit_Measurements_unit_measurement_id",
                table: "Recipe_Ingredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Unit_Measurements",
                table: "Unit_Measurements");

            migrationBuilder.RenameTable(
                name: "Unit_Measurements",
                newName: "unit_measurement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_unit_measurement",
                table: "unit_measurement",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Ingredients_unit_measurement_unit_measurement_id",
                table: "Recipe_Ingredients",
                column: "unit_measurement_id",
                principalTable: "unit_measurement",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Ingredients_unit_measurement_unit_measurement_id",
                table: "Recipe_Ingredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_unit_measurement",
                table: "unit_measurement");

            migrationBuilder.RenameTable(
                name: "unit_measurement",
                newName: "Unit_Measurements");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Unit_Measurements",
                table: "Unit_Measurements",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Ingredients_Unit_Measurements_unit_measurement_id",
                table: "Recipe_Ingredients",
                column: "unit_measurement_id",
                principalTable: "Unit_Measurements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
