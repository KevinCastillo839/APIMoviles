using api.Dtos.Ingredient;
using api.Models;

namespace api.Dtos.Recipe
{
    public class RecipeIngredientDto
    {
        public int id { get; set; }
        public int ingredient_id { get; set; }
        public int recipe_id { get; set; }

        public int unit_measurement_id { get; set; } //
        public IngredientDto Ingredient { get; set; }  // Relación con Ingredient
        public decimal quantity { get; set; } // Cambiado a double para permitir decimales//
    }
}

