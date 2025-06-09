using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuantityField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menu_Users_user_id",
                table: "Menu");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_recipes_Menu_menu_id",
                table: "menu_recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_weekly_menu_Menu_menu_id",
                table: "weekly_menu");

            migrationBuilder.DropForeignKey(
                name: "FK_Weekly_Menu_Table_Users_Userid",
                table: "Weekly_Menu_Table");

            migrationBuilder.DropIndex(
                name: "IX_Weekly_Menu_Table_Userid",
                table: "Weekly_Menu_Table");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Menu",
                table: "Menu");

            migrationBuilder.DropColumn(
                name: "Userid",
                table: "Weekly_Menu_Table");

            migrationBuilder.RenameTable(
                name: "Menu",
                newName: "menu");

            migrationBuilder.RenameIndex(
                name: "IX_Menu_user_id",
                table: "menu",
                newName: "IX_menu_user_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "quantity",
                table: "Recipe_Ingredients",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "unit_measurement_id",
                table: "Recipe_Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Recipeid",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_menu",
                table: "menu",
                column: "id");

            migrationBuilder.CreateTable(
                name: "shopping_list",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    menu_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopping_list", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Unit_Measurements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit_Measurements", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_Ingredients_unit_measurement_id",
                table: "Recipe_Ingredients",
                column: "unit_measurement_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Recipeid",
                table: "Ingredients",
                column: "Recipeid");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_Recipeid",
                table: "Ingredients",
                column: "Recipeid",
                principalTable: "Recipes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_Users_user_id",
                table: "menu",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_recipes_menu_menu_id",
                table: "menu_recipes",
                column: "menu_id",
                principalTable: "menu",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Ingredients_Unit_Measurements_unit_measurement_id",
                table: "Recipe_Ingredients",
                column: "unit_measurement_id",
                principalTable: "Unit_Measurements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_weekly_menu_menu_menu_id",
                table: "weekly_menu",
                column: "menu_id",
                principalTable: "menu",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_Recipeid",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_Users_user_id",
                table: "menu");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_recipes_menu_menu_id",
                table: "menu_recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Ingredients_Unit_Measurements_unit_measurement_id",
                table: "Recipe_Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_weekly_menu_menu_menu_id",
                table: "weekly_menu");

            migrationBuilder.DropTable(
                name: "shopping_list");

            migrationBuilder.DropTable(
                name: "Unit_Measurements");

            migrationBuilder.DropIndex(
                name: "IX_Recipe_Ingredients_unit_measurement_id",
                table: "Recipe_Ingredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_menu",
                table: "menu");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_Recipeid",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "unit_measurement_id",
                table: "Recipe_Ingredients");

            migrationBuilder.DropColumn(
                name: "Recipeid",
                table: "Ingredients");

            migrationBuilder.RenameTable(
                name: "menu",
                newName: "Menu");

            migrationBuilder.RenameIndex(
                name: "IX_menu_user_id",
                table: "Menu",
                newName: "IX_Menu_user_id");

            migrationBuilder.AddColumn<int>(
                name: "Userid",
                table: "Weekly_Menu_Table",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "quantity",
                table: "Recipe_Ingredients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Menu",
                table: "Menu",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Weekly_Menu_Table_Userid",
                table: "Weekly_Menu_Table",
                column: "Userid");

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Users_user_id",
                table: "Menu",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_recipes_Menu_menu_id",
                table: "menu_recipes",
                column: "menu_id",
                principalTable: "Menu",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_weekly_menu_Menu_menu_id",
                table: "weekly_menu",
                column: "menu_id",
                principalTable: "Menu",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Weekly_Menu_Table_Users_Userid",
                table: "Weekly_Menu_Table",
                column: "Userid",
                principalTable: "Users",
                principalColumn: "id");
        }
    }
}
