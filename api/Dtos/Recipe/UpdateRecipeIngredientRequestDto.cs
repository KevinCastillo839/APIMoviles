using System;
using System.Collections.Generic;
using api.Dtos.Ingredient;

namespace api.Dtos.Recipe
{
    public class UpdateRecipeIngredientRequestDto
    {
        public int id { get; set; }
        public int ingredient_id { get; set; }
        public int recipe_id { get; set; }
        public string quantity { get; set; } = 0; // Cambiado a double para permitir decimales
        public DateTime? updated_at { get; set; }

        public IngredientDto Ingredient { get; set; }
    }
}
