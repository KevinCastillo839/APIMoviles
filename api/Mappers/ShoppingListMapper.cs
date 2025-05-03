using api.Dtos.ShoppingList;
using api.Models;
using Api.Dtos.ShoppingList;

namespace api.Mappers
{
  public static class ShoppingListMapper
  {
    public static ShoppingListDto ToDto(this ShoppingList shoppingListItem)
    {
      return new ShoppingListDto
      {
        id = shoppingListItem.id,
        user_id = shoppingListItem.user_id,
        menu_id = shoppingListItem.menu_id,
        created_at = shoppingListItem.created_at,
        updated_at = shoppingListItem.updated_at

      };
    }
    public static ShoppingList ToShoppingListFromCreateDto(this CreateShoppingListRequestDto createUserRequest)
    {
      return new ShoppingList
      {
        user_id = createUserRequest.user_id,
        menu_id = createUserRequest.menu_id,
        created_at = createUserRequest.created_at,
        updated_at = createUserRequest.updated_at
      };
    }
  }
}