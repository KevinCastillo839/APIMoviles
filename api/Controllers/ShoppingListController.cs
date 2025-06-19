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
        
        [HttpGet("by-user/{userId}")]
public async Task<IActionResult> GetWeeklyShoppingList(int userId)
{
    try
    {
        // 1. Obtener el último menú semanal del usuario desde Weekly_Menu_Table
        var latestWeeklyMenu = await _context.Weekly_Menu_Table
            .Where(wmt => wmt.user_id == userId)
            .OrderByDescending(wmt => wmt.created_at)
            .FirstOrDefaultAsync();

        if (latestWeeklyMenu == null)
            return NotFound(new { message = "No se encontró menú semanal para este usuario." });

        // 2. Obtener los menu_id vinculados a ese menú semanal desde weekly_menu
        var menuIds = await _context.weekly_menu
            .Where(wm => wm.menu_table_id == latestWeeklyMenu.id)
            .Select(wm => wm.menu_id)
            .Distinct()
            .ToListAsync();

        if (!menuIds.Any())
            return NotFound(new { message = "No se encontraron menús diarios asociados al menú semanal." });

        // 3. Obtener los recipe_id vinculados a esos menu_id desde menu_recipes
        var recipeIds = await _context.menu_recipes
            .Where(mr => menuIds.Contains(mr.menu_id))
            .Select(mr => mr.recipe_id)
            .Distinct()
            .ToListAsync();

        if (!recipeIds.Any())
            return NotFound(new { message = "No se encontraron recetas asociadas al menú semanal." });

        // 4. Obtener ingredientes de las recetas
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

        // 5. Obtener nombres de ingredientes
        var ingredientIds = recipeIngredientsData.Select(ri => ri.IngredientId).Distinct().ToList();
        var ingredients = await _context.Ingredients
            .Where(i => ingredientIds.Contains(i.id))
            .Select(i => new { i.id, i.name })
            .ToDictionaryAsync(i => i.id, i => i.name);

        // 6. Obtener nombres de unidades de medida
        var unitIds = recipeIngredientsData.Select(ri => ri.UnitId).Distinct().ToList();
        var units = await _context.Unit_Measurements
            .Where(u => unitIds.Contains(u.id))
            .Select(u => new { u.id, u.name })
            .ToDictionaryAsync(u => u.id, u => u.name);

        // 7. Agrupar por ingrediente + unidad y sumar cantidades
        var groupedShoppingList = recipeIngredientsData
            .GroupBy(ri => new { ri.IngredientId, ri.UnitId })
            .Select(g => new
            {
                Ingredient = ingredients.GetValueOrDefault(g.Key.IngredientId, "Ingrediente desconocido"),
                Unit = units.GetValueOrDefault(g.Key.UnitId, "Unidad desconocida"),
                TotalQuantity = g.Sum(ri => ri.Quantity)
            })
            .OrderBy(g => g.Ingredient)
            .ToList();

        // 8. Retornar resultado
        return Ok(new
        {
            success = true,
            data = groupedShoppingList,
            totalItems = groupedShoppingList.Count
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error en GetWeeklyShoppingList: {ex.Message}");
        return StatusCode(500, new
        {
            success = false,
            message = "Error interno del servidor",
            error = ex.Message
        });
    }
}
       /* [HttpGet("by-user/{userId}")]
 public async Task<IActionResult> GetWeeklyShoppingList(int userId)
 {
     try
     {
         // 1. Obtener el último menú semanal del usuario
         var latestWeeklyMenu = await _context.Weekly_Menu_Table
             .Where(wmt => wmt.user_id == userId)
             .OrderByDescending(wmt => wmt.created_at)
             .FirstOrDefaultAsync();

         if (latestWeeklyMenu == null)
             return NotFound(new { message = "No se encontró menú semanal para este usuario." });

         // 2. Obtener los menús diarios vinculados a ese menú semanal
  var recipeIds = await _context.menu_recipes
     .Where(mr => mr.menu_id == latestWeeklyMenu.id)
     .Select(mr => mr.recipe_id)
     .Distinct()
     .ToListAsync();

        if (!recipeIds.Any())
     return NotFound(new { message = "No se encontraron recetas asociadas al menú semanal." });

 // 3. Obtener ingredientes de las recetas (igual que antes)
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

         // 5. Obtener nombres de ingredientes
         var ingredientIds = recipeIngredientsData.Select(ri => ri.IngredientId).Distinct().ToList();
         var ingredients = await _context.Ingredients
             .Where(i => ingredientIds.Contains(i.id))
             .Select(i => new { i.id, i.name})
             .ToDictionaryAsync(i => i.id, i => i.name);

         // 6. Obtener nombres de unidades de medida
         var unitIds = recipeIngredientsData.Select(ri => ri.UnitId).Distinct().ToList();
         var units = await _context.Unit_Measurements
             .Where(u => unitIds.Contains(u.id))
             .Select(u => new { u.id, u.name })
             .ToDictionaryAsync(u => u.id, u => u.name);

         // 7. Agrupar por ingrediente + unidad y sumar cantidades
         var groupedShoppingList = recipeIngredientsData
             .GroupBy(ri => new { ri.IngredientId, ri.UnitId })
             .Select(g => new
             {
                 Ingredient = ingredients.GetValueOrDefault(g.Key.IngredientId, "Ingrediente desconocido"),
                 Unit = units.GetValueOrDefault(g.Key.UnitId, "Unidad desconocida"),
                 TotalQuantity = g.Sum(ri => ri.Quantity)
             })
             .OrderBy(g => g.Ingredient)
             .ToList();

         // 8. Retornar resultado
         return Ok(new
         {
             success = true,
             data = groupedShoppingList,
             totalItems = groupedShoppingList.Count
         });
     }
     catch (Exception ex)
     {
         Console.WriteLine($"Error en GetWeeklyShoppingList: {ex.Message}");
         return StatusCode(500, new
         {
             success = false,
             message = "Error interno del servidor",
             error = ex.Message
         });
     }
 }
 */

        /* [HttpGet("by-user/{userId}")]
            public async Task<IActionResult> GetWeeklyShoppingList(int userId)
            {
                try
                {
                    // 1. Obtener el último menú semanal para el usuario
                    var latestWeeklyMenu = await _context.Weekly_Menu_Table
                        .Where(wmt => wmt.user_id == userId)
                        .OrderByDescending(wmt => wmt.created_at)
                        .FirstOrDefaultAsync();

                    if (latestWeeklyMenu == null)
                        return NotFound(new { message = "No se encontró menú semanal para este usuario." });

                    var latestMenuId = latestWeeklyMenu.id;

                    // 2. Obtener IDs de recetas asociadas al menú
                    var recipeIds = await _context.menu_recipes
                        .Where(mr => mr .menu_id== latestMenuId)
                        .Select(mr => mr.recipe_id)
                        .Distinct()
                        .ToListAsync();

                    if (!recipeIds.Any())
                        return NotFound(new { message = "No se encontraron recetas asociadas al último menú semanal del usuario." });

                    // 3. Obtener ingredientes de las recetas
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

                    // 4. Obtener nombres de ingredientes
                    var ingredientIds = recipeIngredientsData.Select(ri => ri.IngredientId).Distinct().ToList();
                    var ingredients = await _context.Ingredients
                        .Where(i => ingredientIds.Contains(i.id))
                        .Select(i => new { i.id, i.name })
                        .ToDictionaryAsync(i => i.id, i => i.name);

                    // 5. Obtener nombres de unidades de medida
                    var unitIds = recipeIngredientsData.Select(ri => ri.UnitId).Distinct().ToList();
                    var units = await _context.Unit_Measurements
                        .Where(u => unitIds.Contains(u.id))
                        .Select(u => new { u.id, u.name })
                        .ToDictionaryAsync(u => u.id, u => u.name);

                    // 6. Agrupar por ingrediente + unidad y sumar cantidades
                    var groupedShoppingList = recipeIngredientsData
                        .GroupBy(ri => new { ri.IngredientId, ri.UnitId })
                        .Select(g => new
                        {
                            Ingredient = ingredients.GetValueOrDefault(g.Key.IngredientId, "Ingrediente desconocido"),
                            Unit = units.GetValueOrDefault(g.Key.UnitId, "Unidad desconocida"),
                            TotalQuantity = g.Sum(ri => ri.Quantity)
                        })
                        .OrderBy(g => g.Ingredient)
                        .ToList();

                    // 7. Retornar resultado
                    return Ok(new
                    {
                        success = true,
                        data = groupedShoppingList,
                        totalItems = groupedShoppingList.Count
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en GetWeeklyShoppingList: {ex.Message}");
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Error interno del servidor",
                        error = ex.Message
                    });
                }
            }

            */

        /*prueba        
                        [HttpGet("by-user/{userId}")]
                        public async Task<IActionResult> GetWeeklyShoppingList(int userId)
                        {
                            try
                            {
                                // 1. Obtener el ID del menú más reciente desde la tabla weekly_menu, uniendo con Menu para filtrar por user_id
                                var latestMenu = await _context.weekly_menu
                                    .Join(_context.Menu,
                                          wm => wm.menu_id,
                                          m => m.id,
                                          (wm, m) => new { WeeklyMenu = wm, Menu = m })
                                    .Where(joined => joined.Menu.user_id == userId)
                                    .OrderByDescending(joined => joined.WeeklyMenu.created_at)
                                    .Select(joined => joined.WeeklyMenu.menu_id)
                                    .FirstOrDefaultAsync();

                                if (latestMenu == 0)
                                    return NotFound(new { message = "No se encontraron menús semanales para este usuario en la tabla weekly_menu" });

                                // 2. Obtener IDs de recetas del último menú
                                var recipeIds = await _context.menu_recipes
                                    .Where(mr => mr.menu_id == latestMenu)
                                    .Select(mr => mr.recipe_id)
                                    .Distinct()
                                    .ToListAsync();

                                if (!recipeIds.Any())
                                    return NotFound(new { message = "No se encontraron recetas asociadas al último menú semanal del usuario" });

                                // 3. Obtener ingredientes de las recetas SIN Include (evita problemas de mapeo)
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
                                    return NotFound(new { message = "No se encontraron ingredientes para las recetas del último menú semanal" });

                                // 4. Obtener nombres de ingredientes
                                var ingredientIds = recipeIngredientsData.Select(ri => ri.IngredientId).Distinct().ToList();
                                var ingredients = await _context.Ingredients
                                    .Where(i => ingredientIds.Contains(i.id))
                                    .Select(i => new { i.id, i.name })
                                    .ToDictionaryAsync(i => i.id, i => i.name);

                                // 5. Obtener nombres de unidades de medida
                                var unitIds = recipeIngredientsData.Select(ri => ri.UnitId).Distinct().ToList();
                                var units = await _context.Unit_Measurements
                                    .Where(u => unitIds.Contains(u.id))
                                    .Select(u => new { u.id, u.name })
                                    .ToDictionaryAsync(u => u.id, u => u.name);

                                // 6. Agrupar y crear la lista de compras
                                var groupedShoppingList = recipeIngredientsData
                                    .GroupBy(ri => new
                                    {
                                        IngredientId = ri.IngredientId,
                                        UnitId = ri.UnitId
                                    })
                                    .Select(g => new
                                    {
                                        Ingredient = ingredients.GetValueOrDefault(g.Key.IngredientId, "Ingrediente desconocido"),
                                        Unit = units.GetValueOrDefault(g.Key.UnitId, "Unidad desconocida"),
                                        TotalQuantity = g.Sum(ri => ri.Quantity)
                                    })
                                    .OrderBy(g => g.Ingredient)
                                    .ToList();

                                return Ok(new
                                {
                                    success = true,
                                    data = groupedShoppingList,
                                    totalItems = groupedShoppingList.Count
                                });
                            }
                            catch (Exception ex)
                            {
                                // Log el error para debugging
                                Console.WriteLine($"Error en GetWeeklyShoppingList: {ex.Message}");
                                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                                return StatusCode(500, new
                                {
                                    success = false,
                                    message = "Error interno del servidor",
                                    error = ex.Message
                                });
                            }
                        }*/

        /*[HttpGet("by-user/{userId}")]
                        public async Task<IActionResult> GetWeeklyShoppingList(int userId)
                        {
                            try
                            {
                                // 1. Obtener el ID del último menú del usuario
                                var latestMenu = await _context.Menu
                                    .Where(m => m.user_id == userId)
                                    .OrderByDescending(m => m.created_at) // Ordenar por fecha de creación para obtener el más reciente
                                    .Select(m => m.id)
                                    .FirstOrDefaultAsync();

                                if (latestMenu == 0)
                                    return NotFound(new { message = "No se encontraron menús para este usuario" });

                                // 2. Obtener IDs de recetas del último menú
                                var recipeIds = await _context.menu_recipes
                                    .Where(mr => mr.menu_id == latestMenu)
                                    .Select(mr => mr.recipe_id)
                                    .Distinct()
                                    .ToListAsync();

                                if (!recipeIds.Any())
                                    return NotFound(new { message = "No se encontraron recetas asociadas al último menú del usuario" });

                                // 3. Obtener ingredientes de las recetas SIN Include (evita problemas de mapeo)
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
                                    return NotFound(new { message = "No se encontraron ingredientes para las recetas del último menú" });

                                // 4. Obtener nombres de ingredientes
                                var ingredientIds = recipeIngredientsData.Select(ri => ri.IngredientId).Distinct().ToList();
                                var ingredients = await _context.Ingredients
                                    .Where(i => ingredientIds.Contains(i.id))
                                    .Select(i => new { i.id, i.name })
                                    .ToDictionaryAsync(i => i.id, i => i.name);

                                // 5. Obtener nombres de unidades de medida
                                var unitIds = recipeIngredientsData.Select(ri => ri.UnitId).Distinct().ToList();
                                var units = await _context.Unit_Measurements
                                    .Where(u => unitIds.Contains(u.id))
                                    .Select(u => new { u.id, u.name })
                                    .ToDictionaryAsync(u => u.id, u => u.name);

                                // 6. Agrupar y crear la lista de compras
                                var groupedShoppingList = recipeIngredientsData
                                    .GroupBy(ri => new
                                    {
                                        IngredientId = ri.IngredientId,
                                        UnitId = ri.UnitId
                                    })
                                    .Select(g => new
                                    {
                                        Ingredient = ingredients.GetValueOrDefault(g.Key.IngredientId, "Ingrediente desconocido"),
                                        Unit = units.GetValueOrDefault(g.Key.UnitId, "Unidad desconocida"),
                                        TotalQuantity = g.Sum(ri => ri.Quantity)
                                    })
                                    .OrderBy(g => g.Ingredient)
                                    .ToList();

                                return Ok(new
                                {
                                    success = true,
                                    data = groupedShoppingList,
                                    totalItems = groupedShoppingList.Count
                                });
                            }
                            catch (Exception ex)
                            {
                                // Log el error para debugging
                                Console.WriteLine($"Error en GetWeeklyShoppingList: {ex.Message}");
                                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                                return StatusCode(500, new
                                {
                                    success = false,
                                    message = "Error interno del servidor",
                                    error = ex.Message
                                });
                            }
                        }
                        ++++++++
                        */
        /* [HttpGet("by-user/{userId}")]
         public async Task<IActionResult> GetWeeklyShoppingList(int userId)
         {
             try
             {
                 // 1. Obtener IDs de menús del usuario
                 var menuIds = await _context.Menu
                     .Where(m => m.user_id == userId)
                     .Select(m => m.id)
                     .ToListAsync();

                 if (!menuIds.Any())
                     return NotFound(new { message = "No se encontraron menús para este usuario" });

                 // 2. Obtener IDs de recetas de esos menús
                 var recipeIds = await _context.menu_recipes
                     .Where(mr => menuIds.Contains(mr.menu_id))
                     .Select(mr => mr.recipe_id)
                     .Distinct()
                     .ToListAsync();

                 if (!recipeIds.Any())
                     return NotFound(new { message = "No se encontraron recetas asociadas a los menús del usuario" });

                 // 3. Obtener ingredientes de las recetas SIN Include (evita problemas de mapeo)
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
                     return NotFound(new { message = "No se encontraron ingredientes para las recetas" });

                 // 4. Obtener nombres de ingredientes
                 var ingredientIds = recipeIngredientsData.Select(ri => ri.IngredientId).Distinct().ToList();
                 var ingredients = await _context.Ingredients
                     .Where(i => ingredientIds.Contains(i.id))
                     .Select(i => new { i.id, i.name })
                     .ToDictionaryAsync(i => i.id, i => i.name);

                 // 5. Obtener nombres de unidades de medida
                 var unitIds = recipeIngredientsData.Select(ri => ri.UnitId).Distinct().ToList();
                 var units = await _context.Unit_Measurements
                     .Where(u => unitIds.Contains(u.id))
                     .Select(u => new { u.id, u.name })
                     .ToDictionaryAsync(u => u.id, u => u.name);

                 // 6. Agrupar y crear la lista de compras
                 var groupedShoppingList = recipeIngredientsData
                     .GroupBy(ri => new
                     {
                         IngredientId = ri.IngredientId,
                         UnitId = ri.UnitId
                     })
                     .Select(g => new
                     {
                         Ingredient = ingredients.GetValueOrDefault(g.Key.IngredientId, "Ingrediente desconocido"),
                         Unit = units.GetValueOrDefault(g.Key.UnitId, "Unidad desconocida"),
                         TotalQuantity = g.Sum(ri => ri.Quantity)
                     })
                     .OrderBy(g => g.Ingredient)
                     .ToList();

                 return Ok(new
                 {
                     success = true,
                     data = groupedShoppingList,
                     totalItems = groupedShoppingList.Count
                 });
             }
             catch (Exception ex)
             {
                 // Log el error para debugging
                 Console.WriteLine($"Error en GetWeeklyShoppingList: {ex.Message}");
                 Console.WriteLine($"Stack trace: {ex.StackTrace}");

                 return StatusCode(500, new
                 {
                     success = false,
                     message = "Error interno del servidor",
                     error = ex.Message
                 });
             }
         }*/
        // ─────────────────────────────────────────────────────────────────────────────
        // Crear una nueva lista de comprasl
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

        //-----------------
        [HttpPost("generate-list-from-menus")]
        public async Task<IActionResult> GenerateListFromSelectedMenus([FromBody] List<int> menuIds)
        {
            var table = new DataTable();
            table.Columns.Add("MenuId", typeof(int));

            foreach (var id in menuIds)
            {
                table.Rows.Add(id);
            }

            var parameter = new SqlParameter("@MenuIds", table)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.MenuIdTable"
            };

            var result = await _context.SimpleShoppingListItems
                .FromSqlRaw("EXEC GenerateShoppingListByMenus @MenuIds", parameter)
                .ToListAsync();

            if (!result.Any())
                return NotFound(new { message = "No se encontraron ingredientes para los menús seleccionados." });

            return Ok(result);
        }


           [HttpGet("generate-list-from-user/{userId}")]
           public async Task<IActionResult> GenerateListFromUser(int userId)
           {
               var parameter = new SqlParameter("@UserId", userId);

               var result = await _context.SimpleShoppingListItems
                   .FromSqlRaw("EXEC GenerateShoppingListByUserWeeklyMenus @UserId", parameter)
                   .ToListAsync();

               if (!result.Any())
                   return NotFound(new { message = "No se encontraron ingredientes para este usuario." });

               return Ok(result);
           }

       }
}



