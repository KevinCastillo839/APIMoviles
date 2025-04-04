using api.Models;

namespace api.Dtos.Recipe
{
  public class CreateMenuRecipeDto
  {
        public int id { get; set; }
        public int menu_id { get; set; }
        public int recipe_id { get; set; }
        public string meal_time { get; set; } = string.Empty;
     public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime? updated_at { get; set; }
  }
}