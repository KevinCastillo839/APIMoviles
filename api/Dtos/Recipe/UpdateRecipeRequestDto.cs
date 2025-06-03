using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Recipe;

namespace api.Dtos.Ingredient
{
    public class UpdateRecipeRequestDto
    {
        public string name { get; set; } = string.Empty;
        public string instructions { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public int preparation_time { get; set; }
        public IFormFile image { get; set; } 
        public DateTime? updated_at { get; set; }
        
        // Lista de ingredientes que se actualizarán junto con la receta
    public string Recipe_IngredientsJson { get; set; }
    }
}
