using System;
using System.Collections.Generic;
using api.Dtos.Ingredient;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Recipe
{
    public class UpdateRecipeIngredientRequestDto
    {
        
        public int id { get; set; }

        public int ingredient_id { get; set; }

        public int recipe_id { get; set; }
    
        public decimal quantity { get; set; } 
       
        [Required(ErrorMessage = "La fecha de actualización es obligatoria.")]
        public DateTime? updated_at { get; set; }

        [Required(ErrorMessage = "El ingrediente es obligatorio.")]
        public IngredientDto Ingredient { get; set; }
    }
}
