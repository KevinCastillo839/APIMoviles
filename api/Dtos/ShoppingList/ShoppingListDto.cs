using Api.Models;

namespace api.Dtos.ShoppingList
{
  public class ShoppingListDto
  {     
        public int user_id { get; set; }
        public int menu_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
   
}
}