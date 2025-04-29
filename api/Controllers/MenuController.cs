using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Recipe;
using api.Mappers;
using api.Models;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  [Route("api/menu")]
  [ApiController]
  public class MenuController : ControllerBase
  {
    private readonly ApplicationDBContext _context;
    public MenuController(ApplicationDBContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var menus = await _context.Menu.ToListAsync();
      var menusDto = menus.Select(menus => menus.ToDto());
      return Ok(menusDto);
    }
   [HttpGet("all-recipes")]
  public async Task<IActionResult> GetAllr()
  {
    try
    {
        var menus = await _context.Menu
            .Include(m => m.Menu_Recipes)
            .ThenInclude(mr => mr.Recipe)
            .ToListAsync();

        var menusDto = menus.Select(menu => new MenuDto
        {
            id = menu.id,
            name = menu.name,
            description = menu.description,
            day_of_week = menu.day_of_week,
            Menu_Recipes = menu.Menu_Recipes.Select(mr => new MenuRecipeDto
            {
                id = mr.id,
                meal_time = mr.meal_time,
                recipe = new RecipeDto
                {
                    id = mr.Recipe.id,
                    name = mr.Recipe.name,
                    category = mr.Recipe.category,
                    preparation_time = mr.Recipe.preparation_time,
                    image_url = mr.Recipe.image_url
                }
            }).ToList()
        }).ToList();

        return Ok(menusDto);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error en el servidor: {ex.Message}");
    }
}
  [HttpGet("user/{userId}")]
public async Task<IActionResult> GetByUser(int userId)
{
    try
    {
        var menus = await _context.Menu
            .Where(m => m.user_id == userId)
            .Include(m => m.Menu_Recipes)
            .ThenInclude(mr => mr.Recipe)
            .ToListAsync();

        var menusDto = menus.Select(menu => new MenuDto
        {
            id = menu.id,
            name = menu.name,
            description = menu.description,
            day_of_week = menu.day_of_week,
            Menu_Recipes = menu.Menu_Recipes.Select(mr => new MenuRecipeDto
            {
                id = mr.id,
                menu_id = mr.menu_id,
                recipe_id= mr.recipe_id,
                meal_time = mr.meal_time,
                recipe = new RecipeDto
                {
                    id = mr.Recipe.id,
                    name = mr.Recipe.name,
                    category = mr.Recipe.category,
                    preparation_time = mr.Recipe.preparation_time,
                    image_url = mr.Recipe.image_url
                }
            }).ToList()
        }).ToList();

        return Ok(menusDto);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error en el servidor: {ex.Message}");
    }
}
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    try
    {
        var menu = await _context.Menu
            .Include(m => m.Menu_Recipes)
            .ThenInclude(mr => mr.Recipe)
            .FirstOrDefaultAsync(m => m.id == id);

        if (menu == null)
            return NotFound("Menú no encontrado");

        var menuDto = new MenuDto
        {
            id = menu.id,
            name = menu.name,
            description = menu.description,
            day_of_week = menu.day_of_week,
            Menu_Recipes = menu.Menu_Recipes.Select(mr => new MenuRecipeDto
            {
                id = mr.id,
                menu_id= mr.menu_id,
                recipe_id= mr.recipe_id,
                meal_time = mr.meal_time,
                recipe = new RecipeDto
                {
                    id = mr.Recipe.id,
                    name = mr.Recipe.name,
                    category = mr.Recipe.category,
                    preparation_time = mr.Recipe.preparation_time,
                    image_url = mr.Recipe.image_url
                }
            }).ToList()
        };

        return Ok(menuDto);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error en el servidor: {ex.Message}");
    }
}
[HttpPost]
public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto menuDto)
{
    try
    {
        // Crear menú principal
        var menu = new Menu
        {
            name = menuDto.name,
            description = menuDto.description,
            day_of_week = menuDto.day_of_week,
            user_id = menuDto.user_id,
           created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        _context.Menu.Add(menu);
        await _context.SaveChangesAsync(); // Aquí ya se genera menu.id

        // Relacionar recetas existentes con el menú
        foreach (var mrDto in menuDto.Menu_Recipes)
        {
            // Validar que la receta exista
            var recipeExists = await _context.Recipes.AnyAsync(r => r.id == mrDto.recipe_id);
            if (!recipeExists)
            {
                return BadRequest($"La receta con ID {mrDto.recipe_id} no existe.");
            }

            var menuRecipe = new Menu_Recipes
            {
                menu_id = menu.id, // Ya generado por EF
                recipe_id = mrDto.recipe_id,
                meal_time = mrDto.meal_time,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            _context.menu_recipes.Add(menuRecipe);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Menú creado exitosamente", menu_id = menu.id });
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al crear el menú: {ex.InnerException?.Message ?? ex.Message}");
    }
}


  [HttpDelete("{menuId}")]
public async Task<IActionResult> DeleteMenu(int menuId)
{
    try
    {
        // Buscar el menú con las recetas asociadas
        var menu = await _context.Menu
            .Include(m => m.Menu_Recipes)
            .FirstOrDefaultAsync(m => m.id == menuId);

        if (menu == null)
        {
            return NotFound("El menú no existe.");
        }

        // Eliminar las relaciones en la tabla 'menu_recipes'
        _context.menu_recipes.RemoveRange(menu.Menu_Recipes);

        // Eliminar el menú
        _context.Menu.Remove(menu);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Menú y sus recetas asociadas eliminadas exitosamente." });
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al eliminar el menú: {ex.Message}");
    }
}
[HttpPut("{menuId}")]
public async Task<IActionResult> UpdateMenu(int menuId, [FromBody] UpdateMenuDto menuDto)
{
    try
    {
        // Buscar el menú con sus recetas asociadas
        var menu = await _context.Menu
            .Include(m => m.Menu_Recipes)
            .FirstOrDefaultAsync(m => m.id == menuId);

        if (menu == null)
        {
            return NotFound("El menú no existe.");
        }

        // Actualizar los datos del menú
        menu.name = menuDto.name;
        menu.description = menuDto.description;
        menu.day_of_week = menuDto.day_of_week;
        menu.user_id = menuDto.user_id;

        // Validar fechas si se van a usar (opcional)
    

        // Procesar recetas asociadas (sin eliminar las existentes)
        foreach (var mrDto in menuDto.Menu_Recipes)
        {
            var existingRecipe = menu.Menu_Recipes
                .FirstOrDefault(mr => mr.recipe_id == mrDto.recipe_id);

            if (existingRecipe == null)
            {
                var menuRecipe = new Menu_Recipes
                {
                    menu_id = menu.id,
                    recipe_id = mrDto.recipe_id,
                    meal_time = mrDto.meal_time,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };

                _context.menu_recipes.Add(menuRecipe);
            }
            else
            {
                existingRecipe.meal_time = mrDto.meal_time;
                existingRecipe.updated_at = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Menú actualizado exitosamente" });
    }
    catch (Exception ex)
    {
        var errorMessage = ex.Message;

        if (ex.InnerException != null)
        {
            errorMessage += " | Inner Exception: " + ex.InnerException.Message;
        }

        return StatusCode(500, $"Error al actualizar el menú: {errorMessage}");
    }
}



[HttpGet("weekly/user/{userId}")]
public async Task<IActionResult> GetWeeklyMenuTables(int userId)
{
    try
    {
        var menuTables = await _context.Weekly_Menu_Table
            .Where(wmt => wmt.user_id == userId)
            .Include(wmt => wmt.Weekly_Menus)
                .ThenInclude(wm => wm.Menu)
                    .ThenInclude(m => m.Menu_Recipes)
                        .ThenInclude(mr => mr.Recipe)
            .ToListAsync();

        var result = menuTables.Select(wmt => new
        {
            id = wmt.id,
            created_at = wmt.created_at,
            weekly_menus = wmt.Weekly_Menus.Select(wm => new
            {
                id = wm.id,
                day_of_week = wm.day_of_week,
                menu = new
                {
                    id = wm.Menu.id,
                    name = wm.Menu.name,
                    description = wm.Menu.description,
                    day_of_week = wm.Menu.day_of_week,
                    recipes = wm.Menu.Menu_Recipes.Select(mr => new
                    {
                        id = mr.Recipe.id,
                        name = mr.Recipe.name,
                        category = mr.Recipe.category,
                        preparation_time = mr.Recipe.preparation_time,
                        image_url = mr.Recipe.image_url
                    })
                }
            })
        });

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error en el servidor: {ex.Message}");
    }
}





  



  }
}
