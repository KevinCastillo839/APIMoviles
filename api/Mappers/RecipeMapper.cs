using api.Dtos.Recipe;
using api.Models;

namespace api.Mappers
{
  public static class RecipeMapper
  {
    public static RecipeDto ToDto(this Recipe recipeItem)
    {
      return new RecipeDto
      {
        id = recipeItem.id,
        name = recipeItem.name,
        instructions = recipeItem.instructions,
        category = recipeItem.category,
        preparation_time= recipeItem.preparation_time,
        image_url = recipeItem.image_url,
        created_at = recipeItem.created_at,
        updated_at = recipeItem.updated_at

      };
    }
  }
}