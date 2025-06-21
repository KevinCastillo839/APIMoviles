using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class FixShoppingListRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "recipe_id",
                table: "shopping_list",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_shopping_list_recipe_id",
                table: "shopping_list",
                column: "recipe_id");

            migrationBuilder.AddForeignKey(
                name: "FK_shopping_list_Recipes_recipe_id",
                table: "shopping_list",
                column: "recipe_id",
                principalTable: "Recipes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shopping_list_Recipes_recipe_id",
                table: "shopping_list");

            migrationBuilder.DropIndex(
                name: "IX_shopping_list_recipe_id",
                table: "shopping_list");

            migrationBuilder.DropColumn(
                name: "recipe_id",
                table: "shopping_list");
        }
    }
}
