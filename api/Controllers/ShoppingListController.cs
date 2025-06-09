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

        // ─────────────────────────────────────────────────────────────────────────────
        // Obtener la lista de compras agrupando ingredientes por nombre y unidad
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpGet("by-menu/{menuId}")]
        public async Task<IActionResult> GetShoppingListByMenu(int menuId)
        {
            var shoppingLists = await _context.ShoppingLists
                .Where(sl => sl.menu_id == menuId)
                .OrderBy(sl => sl.created_at)
                .ToListAsync();

            if (!shoppingLists.Any())
                return NotFound(new { message = "No se encontraron listas de compras para el menú seleccionado" });

            var recipeIds = await _context.menu_recipes
                .Where(mr => mr.menu_id == menuId)
                .Select(mr => mr.recipe_id)
                .ToListAsync();

            if (!recipeIds.Any())
                return NotFound(new { message = "No se encontraron recetas asociadas a este menú" });

            var recipeIngredients = await _context.Recipe_Ingredients
                .Include(ri => ri.Ingredient)
                .Include(ri => ri.Unit_Measurement)
                .Where(ri => recipeIds.Contains(ri.RecipeId))
                .ToListAsync();

            var groupedShoppingList = recipeIngredients
                .GroupBy(ri => new { IngredientName = ri.Ingredient.name, UnitName = ri.Unit_Measurement.name })
                .Select(g => new
                {
                    IngredientName = g.Key.IngredientName,
                    Unit = g.Key.UnitName,
                    TotalQuantity = g.Sum(ri => ri.quantity)
                })
                .OrderBy(g => g.IngredientName)
                .ToList();

            return Ok(groupedShoppingList);
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Obtener la lista de compras para un usuario y menú específico
        // ─────────────────────────────────────────────────────────────────────────────
            [HttpGet("by-user/{userId}/menu/{menuId}")]
        public async Task<IActionResult> GetShoppingListByUserAndMenu(int userId, int menuId)
        {
            Console.WriteLine($"Buscando listas de compras para userId={userId} y menuId={menuId}");

            var shoppingLists = await _context.ShoppingLists
                .Where(sl => sl.user_id == userId && sl.menu_id == menuId)
                .OrderBy(sl => sl.created_at)
                .ToListAsync();

            if (!shoppingLists.Any())
                return NotFound(new { message = "No se encontraron listas de compras para este usuario y menú" });

            return Ok(shoppingLists);
        }

     
       /* [HttpGet("by-user/{userId}/menu/{menuId}")]
          public async Task<IActionResult> GetShoppingListByUserAndMenu(int userId, int menuId)
          {
              var shoppingLists = await _context.ShoppingLists
                  .Where(sl => sl.user_id == userId && sl.menu_id == menuId)
                  .OrderBy(sl => sl.created_at)
                  .ToListAsync();

              if (!shoppingLists.Any())
                  return NotFound(new { message = "No se encontraron listas de compras para este usuario y menú" });

              return Ok(shoppingLists);
          }
  */
        // ─────────────────────────────────────────────────────────────────────────────
        // Obtener lista de compras semanal para un usuario
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetWeeklyShoppingList(int userId)
        {
            var menuIds = await _context.Menu
                .Where(m => m.user_id == userId)
                .Select(m => m.id)
                .ToListAsync();

            if (!menuIds.Any())
                return NotFound(new { message = "No se encontraron menús para este usuario" });

            var recipeIds = await _context.menu_recipes
                .Where(mr => menuIds.Contains(mr.menu_id))
                .Select(mr => mr.recipe_id)
                .Distinct()
                .ToListAsync();

            if (!recipeIds.Any())
                return NotFound(new { message = "No se encontraron recetas asociadas a los menús del usuario" });

            var recipeIngredients = await _context.Recipe_Ingredients
                .Include(ri => ri.Ingredient)
                .Include(ri => ri.Unit_Measurement)
                .Where(ri => recipeIds.Contains(ri.RecipeId))
                .ToListAsync();

            var groupedShoppingList = recipeIngredients
                .GroupBy(ri => new { IngredientName = ri.Ingredient.name, UnitName = ri.Unit_Measurement.name })
                .Select(g => new
                {
                    Ingredient = g.Key.IngredientName,
                    Unit = g.Key.UnitName,
                    TotalQuantity = g.Sum(ri => ri.quantity)
                })
                .OrderBy(g => g.Ingredient)
                .ToList();

            return Ok(groupedShoppingList);
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Crear una nueva lista de compras
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> CreateShoppingList([FromBody] ShoppingList shoppingList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            shoppingList.created_at = DateTime.UtcNow;
            shoppingList.updated_at = DateTime.UtcNow;

            _context.ShoppingLists.Add(shoppingList);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShoppingListByMenu), new { menuId = shoppingList.menu_id }, shoppingList);
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Actualizar una lista de compras existente
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShoppingList(int id, [FromBody] ShoppingList shoppingList)
        {
            if (id != shoppingList.id)
                return BadRequest(new { message = "El ID de la lista de compras no coincide" });

            var existingShoppingList = await _context.ShoppingLists.FindAsync(id);
            if (existingShoppingList == null)
                return NotFound(new { message = "Lista de compras no encontrada" });

            shoppingList.updated_at = DateTime.UtcNow;
            _context.Entry(existingShoppingList).CurrentValues.SetValues(shoppingList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Eliminar una lista de compras
        // ─────────────────────────────────────────────────────────────────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingList(int id)
        {
            var shoppingList = await _context.ShoppingLists.FindAsync(id);
            if (shoppingList == null)
                return NotFound(new { message = "Lista de compras no encontrada" });

            _context.ShoppingLists.Remove(shoppingList);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}


