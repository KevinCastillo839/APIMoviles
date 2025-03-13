using Api.Models;

namespace api.Dtos.Ingredient
{
  public class IngredientDto
  {     
         public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
   
}
}