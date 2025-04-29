using api.Models;

namespace api.Dtos.Recipe
{
  public class WeeklyMenuDto
  {
         public int id { get; set; }
        public int menu_id { get; set; }
    
        public string day_of_week { get; set; } = string.Empty;
        public Menu Menu { get; set; }


        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public List<MenuDto> weeklymenus { get; set; } = new List<MenuDto>(); // Define correctamente la lista

  }
}