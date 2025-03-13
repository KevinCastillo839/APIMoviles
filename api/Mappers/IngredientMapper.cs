using api.Dtos.Ingredient;
using api.Models;

namespace api.Mappers
{
  public static class IngredientMapper
  {
    public static IngredientDto ToDto(this Ingredient ingredientItem)
    {
      return new IngredientDto
      {
        id = ingredientItem.id,
        name = ingredientItem.name,
        description = ingredientItem.description,
        created_at = ingredientItem.created_at,
        updated_at = ingredientItem.updated_at

      };
    }
     public static Ingredient ToIngredientFromCreateDto(this CreateIngredientRequestDto createUserRequest)
    {
      return new Ingredient
      {
        name = createUserRequest.name,
        description = createUserRequest.description,
        created_at = createUserRequest.created_at,
        updated_at = createUserRequest.updated_at

      };
    }
  }
}