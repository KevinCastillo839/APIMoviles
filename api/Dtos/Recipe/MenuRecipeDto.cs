using api.Models;

namespace api.Dtos.Recipe
{
  public class MenuRecipeDto
  {
        public int id { get; set; }
        public int menu_id { get; set; }
        public int recipe_id { get; set; }
        public string meal_time { get; set; } = string.Empty;
         public RecipeDto recipe { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
  }
}