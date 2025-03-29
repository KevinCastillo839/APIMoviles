using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Recipe;

namespace api.Dtos.Ingredient
{
  public class CreateRecipeRequestDto
  {
     
        public string name { get; set; }
        public string instructions { get; set; }
        public string category { get; set; }
        public int preparation_time { get; set; }
        public string image_url { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow; // Evita valores fuera de rango
         public DateTime? updated_at { get; set; } = null;
        public List<CreateRecipeIngredientDto> Recipe_Ingredients { get; set; } = new List<CreateRecipeIngredientDto>();
  }
}