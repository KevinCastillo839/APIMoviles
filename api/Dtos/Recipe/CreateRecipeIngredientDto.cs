using System;
using System.ComponentModel.DataAnnotations;
using api.Dtos.Ingredient;
using api.Models;

namespace api.Dtos.Recipe
{

    public class CreateRecipeIngredientDto
    {
        public int ingredient_id { get; set; }

        public int recipe_id { get; set; }

        public decimal quantity { get; set; } 


        [Required(ErrorMessage = "La fecha de creación es obligatoria.")]
        public DateTime created_at { get; set; }

        [Required(ErrorMessage = "La fecha de actualización es obligatoria.")]
        public DateTime updated_at { get; set; }
    }
}

