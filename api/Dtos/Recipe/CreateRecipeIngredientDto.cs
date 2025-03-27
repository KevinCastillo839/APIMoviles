using api.Dtos.Ingredient;
using api.Models;

namespace api.Dtos.Recipe
{
 
  public class CreateRecipeIngredientDto
{

    public int ingredient_id { get; set; }
    public int recipe_id { get; set; }
    public string quantity { get; set; }
    
      public DateTime created_at { get; set; }
     public DateTime updated_at { get; set; }
}
}