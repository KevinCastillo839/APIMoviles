using api.Dtos.Recipe;
using api.Models;
using Api.Models;

namespace api.Mappers
{
  public static class WeeklyMenuMapper
  {
    public static WeeklyMenuDto ToDto(this Weekly_Menu menuItem)
    {
      return new WeeklyMenuDto
      {
        id = menuItem.id,
        menu_id = menuItem.menu_id,
        day_of_week = menuItem.day_of_week,
        created_at = menuItem.created_at,
        updated_at = menuItem.updated_at

      };
    }
  }
}