using Api.Models;

namespace api.Dtos.Recipe
{
  public class MenuDto
  {     
         public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string day_of_week { get; set; } = string.Empty;
        public int user_id { get; set; }
      public List<MenuRecipeDto> Menu_Recipes { get; set; } = new List<MenuRecipeDto>(); // Define correctamente la lista
}
}
