using api.Dtos.Recipe;
using api.Models;

namespace api.Mappers
{
  public static class MenuMapper
  {
    public static MenuDto ToDto(this Menu menuItem)
    {
      return new MenuDto
      {
        id = menuItem.id,
        name = menuItem.name,
        description = menuItem.description,
        day_of_week = menuItem.day_of_week,
        user_id= menuItem.user_id,
        created_at = menuItem.created_at,
        updated_at = menuItem.updated_at

      };
    }
  }
}