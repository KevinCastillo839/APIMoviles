using Api.Models;

namespace api.Dtos.Recipe
{
  public class UpdateMenuRecipeDto
  {     
 
    public int menu_id { get; set; }
        public int recipe_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public string meal_time { get; set; } = string.Empty;

        public int user_id { get; set; }
}
}
