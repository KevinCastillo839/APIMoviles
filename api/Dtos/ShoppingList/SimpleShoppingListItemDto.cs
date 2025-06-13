using Api.Models;

namespace api.Dtos.ShoppingList
{
    public class SimpleShoppingListItem
    {
        public string IngredientName { get; set; }
        public string Unit { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}