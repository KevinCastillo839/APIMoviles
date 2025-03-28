using System;
using System.ComponentModel.DataAnnotations;
using api.Dtos.Ingredient;
using api.Models;

namespace api.Dtos.Recipe
{
    public class CreateRecipeIngredientDto
    {
        [Required(ErrorMessage = "El ID del ingrediente es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del ingrediente debe ser un número positivo.")]
        public int ingredient_id { get; set; }

        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la receta debe ser un número positivo.")]
        public int recipe_id { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [StringLength(50, ErrorMessage = "La cantidad no puede tener más de 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9,.:\s]+$", ErrorMessage = "La cantidad solo puede contener letras, números, comas, puntos y espacios.")]
        public string quantity { get; set; } 


        [Required(ErrorMessage = "La fecha de creación es obligatoria.")]
        public DateTime created_at { get; set; }

        [Required(ErrorMessage = "La fecha de actualización es obligatoria.")]
        public DateTime updated_at { get; set; }
    }
}
