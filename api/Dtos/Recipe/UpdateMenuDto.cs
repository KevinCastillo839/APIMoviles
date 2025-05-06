using Api.Models;

namespace api.Dtos.Recipe
{
  public class UpdateMenuDto
  {     
 
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public string day_of_week { get; set; } = string.Empty;
        public int user_id { get; set; }
      public List<CreateMenuRecipeDto> Menu_Recipes { get; set; } = new List<CreateMenuRecipeDto>(); // Define correctamente la lista
}
}
