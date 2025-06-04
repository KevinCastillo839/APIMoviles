using System;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace api.Controllers
{
    [Route("api/shoppinglist")]
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ShoppingListController(ApplicationDBContext context)
        {
            _context = context;
        }

        // Obtener la lista de compras agrupando ingredientes por nombre y unidad
[HttpGet("by-menu/{menuId}")]
public async Task<IActionResult> GetShoppingListByMenu(int menuId)
{
    var shoppingLists = await _context.ShoppingLists
        .Where(sl => sl.menu_id == menuId)
        .OrderBy(sl => sl.created_at)
        .ToListAsync();

    if (!shoppingLists.Any())  // Aquí se verifica que la lista esté vacía
    {
        return NotFound(new { message = "No se encontraron listas de compras para el menú seleccionado" });
    }

    var recipes = await _context.menu_recipes
        .Where(mr => mr.menu_id == menuId)
        .ToListAsync();

    if (!recipes.Any())  // Aquí también se verifica que no haya recetas
    {
        return NotFound(new { message = "No se encontraron recetas asociadas a este menú" });
    }

    var recipeIds = recipes.Select(r => r.recipe_id).ToList();

    var recipeIngredients = await _context.Recipe_Ingredients
        .Include(ri => ri.Ingredient)
        .Include(ri => ri.Unit_Measurement)
        .Where(ri => recipeIds.Contains(ri.recipe_id))
        .ToListAsync();

    var groupedShoppingList = recipeIngredients
        .GroupBy(ri => new { IngredientName = ri.Ingredient.name, UnitName = ri.Unit_Measurement.name })
        .Select(group => new
        {
            IngredientName = group.Key.IngredientName,
            Unit = group.Key.UnitName,
            TotalQuantity = group.Sum(ri => ri.quantity)
        })
        .OrderBy(g => g.IngredientName)
        .ToList();

    return Ok(groupedShoppingList);
}

        // Obtener la lista de compras para un usuario y menú específico
        [HttpGet("by-user/{userId}/menu/{menuId}")]
        public async Task<IActionResult> GetShoppingListByUserAndMenu(int userId, int menuId)
        {
            var shoppingLists = await _context.ShoppingLists
                .Where(sl => sl.user_id == userId && sl.menu_id == menuId)
                .OrderBy(sl => sl.created_at)
                .ToListAsync();

            if (shoppingLists == null || !shoppingLists.Any())
            {
                return NotFound(new { message = "No se encontraron listas de compras para este usuario y menú" });
            }

            return Ok(shoppingLists);
        }

        // Crear una nueva lista de compras
        [HttpPost]
        public async Task<IActionResult> CreateShoppingList([FromBody] ShoppingList shoppingList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            shoppingList.created_at = DateTime.UtcNow;
            shoppingList.updated_at = DateTime.UtcNow;

            _context.ShoppingLists.Add(shoppingList);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShoppingListByMenu), new { menuId = shoppingList.menu_id }, shoppingList);
        }

        // Actualizar una lista de compras
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShoppingList(int id, [FromBody] ShoppingList shoppingList)
        {
            if (id != shoppingList.id)
            {
                return BadRequest(new { message = "El ID de la lista de compras no coincide" });
            }

            var existingShoppingList = await _context.ShoppingLists.FindAsync(id);
            if (existingShoppingList == null)
            {
                return NotFound(new { message = "Lista de compras no encontrada" });
            }

            existingShoppingList.updated_at = DateTime.UtcNow;
            _context.Entry(existingShoppingList).CurrentValues.SetValues(shoppingList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Eliminar una lista de compras
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingList(int id)
        {
            var shoppingList = await _context.ShoppingLists.FindAsync(id);
            if (shoppingList == null)
            {
                return NotFound(new { message = "Lista de compras no encontrada" });
            }

            _context.ShoppingLists.Remove(shoppingList);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
