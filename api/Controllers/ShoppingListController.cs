using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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

        // Retrieve the shopping list by grouping ingredients by name and unit.
     
      
[HttpGet("by-user/{userId}")]
public async Task<IActionResult> GetWeeklyShoppingList(int userId)
{
    try
    {
        // 1. Get the most recent weekly menu.
        var latestWeeklyMenu = await _context.Weekly_Menu_Table
            .Where(wmt => wmt.user_id == userId)
            .OrderByDescending(wmt => wmt.id)
            .FirstOrDefaultAsync();

        if (latestWeeklyMenu == null)
            return NotFound(new { message = "No se encontró menú semanal para este usuario." });

        // 2. Get the menu IDs
        var menuIds = await _context.weekly_menu
            .Where(wm => wm.menu_table_id == latestWeeklyMenu.id)
            .Select(wm => wm.menu_id)
            .Distinct()
            .ToListAsync();

        if (!menuIds.Any())
            return NotFound(new { message = "No se encontraron menús diarios asociados al menú semanal." });

        // 3. Get the recipe IDs
        var recipeIds = await _context.menu_recipes
            .Where(mr => menuIds.Contains(mr.menu_id))
            .Select(mr => mr.recipe_id)
            .Distinct()
            .ToListAsync();

        if (!recipeIds.Any())
            return NotFound(new { message = "No se encontraron recetas asociadas al menú semanal." });

        // 4. Get ingredients
        var recipeIngredientsData = await _context.Recipe_Ingredients
            .Where(ri => recipeIds.Contains(ri.RecipeId))
            .Select(ri => new
            {
                IngredientId = ri.ingredient_id,
                UnitId = ri.unit_measurement_id,
                Quantity = ri.quantity
            })
            .ToListAsync();

        if (!recipeIngredientsData.Any())
            return NotFound(new { message = "No se encontraron ingredientes para las recetas del menú." });

        // 5. Get ingredient names
        var ingredientIds = recipeIngredientsData.Select(ri => ri.IngredientId).Distinct().ToList();
        var ingredients = await _context.Ingredients
            .Where(i => ingredientIds.Contains(i.id))
            .Select(i => new { i.id, i.name })
            .ToDictionaryAsync(i => i.id, i => i.name);

        // 6. Get unit names
        var unitIds = recipeIngredientsData.Select(ri => ri.UnitId).Distinct().ToList();
        var units = await _context.unit_measurement
            .Where(u => unitIds.Contains(u.id))
            .Select(u => new { u.id, u.name })
            .ToDictionaryAsync(u => u.id, u => u.name);

        // Extended validation to identify missing keys
        var missingIngredientIds = ingredientIds.Except(ingredients.Keys).ToList();
        var missingUnitIds = unitIds.Where(id => id.HasValue).Select(id => id.Value).Except(units.Keys).ToList();

        if (missingIngredientIds.Any())
            Console.WriteLine($"IDs de ingredientes faltantes: {string.Join(", ", missingIngredientIds)}");

        if (missingUnitIds.Any())
            Console.WriteLine($"IDs de unidades faltantes: {string.Join(", ", missingUnitIds)}");

        // Group with safeguards against null values
        var groupedShoppingList = recipeIngredientsData
            .GroupBy(ri => new { ri.IngredientId, ri.UnitId })
            .Select(g =>
            {
                string ingredientName = null;
                ingredients.TryGetValue(g.Key.IngredientId, out ingredientName);
                if (string.IsNullOrWhiteSpace(ingredientName))
                    ingredientName = "Ingrediente desconocido";

                string unitName = null;
                units.TryGetValue((int)g.Key.UnitId, out unitName);
                if (string.IsNullOrWhiteSpace(unitName))
                    unitName = "Unidad desconocida";

                decimal totalQuantity = 0;
                try
                {
                    totalQuantity = g.Sum(ri => ri.Quantity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sumando cantidades para ingrediente {ingredientName}: {ex.Message}");
                }

                return new
                {
                    Ingredient = ingredientName,
                    Unit = unitName,
                    TotalQuantity = totalQuantity
                };
            })
            .OrderBy(g => g.Ingredient)
            .ToList();
        // 8. Return result"
        return Ok(new
        {
            success = true,
            data = groupedShoppingList,
            totalItems = groupedShoppingList.Count
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error en GetWeeklyShoppingList: {ex}");
        return StatusCode(500, new
        {
            success = false,
            message = "Error interno del servidor",
            error = ex.Message
        });
    }
}

        // ─────────────────────────────────────────────────────────────────────────────

       }
}



