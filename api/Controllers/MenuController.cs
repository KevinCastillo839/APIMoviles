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
                        recipe_id = mr.recipe_id,
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
                        menu_id = mr.menu_id,
                        recipe_id = mr.recipe_id,
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

        /*
        [HttpGet("weekly/user/{userId}")]
        public async Task<IActionResult> GetLatestWeeklyMenuTable(int userId)
        {
            try
            {
                var latestMenuTable = await _context.Weekly_Menu_Table
                    .Where(wmt => wmt.user_id == userId)
                    .OrderByDescending(wmt => wmt.id)
                    .Include(wmt => wmt.Weekly_Menus)
                        .ThenInclude(wm => wm.Menu)
                            .ThenInclude(m => m.Menu_Recipes)
                                .ThenInclude(mr => mr.Recipe)
                    .FirstOrDefaultAsync();

                if (latestMenuTable == null)
                    return NotFound("No se encontró ningún menú semanal para el usuario.");

                var result = new
                {
                    id = latestMenuTable.id,
                     created_at = latestMenuTable.created_at.ToString("dd/MM/yyyy"),
                    weekly_menus = latestMenuTable.Weekly_Menus.Select(wm => new
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
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }*/
[HttpGet("weekly/user/{userId}")]
public async Task<IActionResult> GetLatestWeeklyMenuTable(int userId)
{
    try
    {
        var latestMenuTable = await _context.Weekly_Menu_Table
            .Where(wmt => wmt.user_id == userId)
            .OrderByDescending(wmt => wmt.id)
            .Include(wmt => wmt.Weekly_Menus)
                .ThenInclude(wm => wm.Menu)
                    .ThenInclude(m => m.Menu_Recipes)
                        .ThenInclude(mr => mr.Recipe)
                            .ThenInclude(r => r.Recipe_Ingredients)
                                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync();

        if (latestMenuTable == null)
            return NotFound("No se encontró ningún menú semanal para el usuario.");

        var result = new
        {
            id = latestMenuTable.id,
            created_at = latestMenuTable.created_at.ToString("dd/MM/yyyy"),
            weekly_menus = latestMenuTable.Weekly_Menus.Select(wm => new
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
                        image_url = mr.Recipe.image_url,
                        recipe_ingredients = mr.Recipe.Recipe_Ingredients.Select(ri => new
                        {
                            id = ri.id,
                            recipe_id = ri.recipe_id,
                            ingredient_id = ri.ingredient_id,
                            quantity = ri.quantity,
                            ingredient = new
                            {
                                id = ri.Ingredient.id,
                                name = ri.Ingredient.name,
                                description = ri.Ingredient.description
                            }
                        })
                    })
                }
            })
        };

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error en el servidor: {ex.Message}");
    }
}


[HttpPost("weekly/user/{user_id}/generate")]
public async Task<IActionResult> GenerateWeeklyMenu(int user_id)
{
    try
    {
        // 1. Get user preferences including goals and restrictions
        var preference = await _context.user_preferences
            .Include(p => p.User_Dietary_Goals)
                .ThenInclude(udg => udg.dietary_Goal)
            .Include(p => p.User_Dietary_Restrictions)
                .ThenInclude(udr => udr.dietary_Restriction)
            .FirstOrDefaultAsync(p => p.user_id == user_id);

        // 2. Get user's allergies (lowercase names)
        var userAllergyNames = await _context.User_Allergies
            .Where(ua => ua.user_id == user_id)
            .Select(ua => ua.Allergy.name.ToLower())
            .ToListAsync();

        // 3. Get all recipes with ingredients
        var recipes = await _context.Recipes
            .Include(r => r.Recipe_Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .ToListAsync();

        // 4. Get all recipe-allergy relationships
        var recipeAllergies = await _context.Recipe_Allergies
            .Include(ra => ra.Allergy)
            .ToListAsync();

        // 5. Get restriction names in lowercase, if preferences exist
        var restrictionNames = preference?.User_Dietary_Restrictions
            .Select(udr => udr.dietary_Restriction.name.ToLower())
            .ToList() ?? new List<string>();

        List<Recipe> validRecipes;

        if (preference != null)
        {
            // Filter recipes based on allergies and restrictions
            validRecipes = recipes.Where(r =>
            {
                var allergiesForRecipe = recipeAllergies
                    .Where(ra => ra.recipe_id == r.id)
                    .Select(ra => ra.Allergy.name.ToLower())
                    .ToList();

                var ingredientNames = r.Recipe_Ingredients.Select(ri => ri.Ingredient.name.ToLower());

                bool containsUserAllergy = allergiesForRecipe.Any(allergy => userAllergyNames.Contains(allergy)) ||
                                           ingredientNames.Any(ing => userAllergyNames.Any(allergy => ing.Contains(allergy)));

                bool containsRestriction = ingredientNames.Any(ing => restrictionNames.Any(rn => ing.Contains(rn)));

                return !containsUserAllergy && !containsRestriction;

                // You can also add goal validation here if needed
                // && ValidateRecipeWithGoals(r, preference.User_Dietary_Goals);
            }).ToList();

            // If no valid recipes found, fallback to all recipes
            if (!validRecipes.Any())
            {
                validRecipes = recipes;
            }
        }
        else
        {
            // If no preferences, use all recipes
            validRecipes = recipes;
        }

        if (!validRecipes.Any())
           return BadRequest(new { message = "No recipes available to generate the menu." });


        // 6. Create Weekly_Menu_Table
        var weeklyMenuTable = new Weekly_Menu_Table
        {
            user_id = user_id,
            created_at = DateTime.UtcNow,
            Weekly_Menus = new List<Weekly_Menu>()
        };

        _context.Weekly_Menu_Table.Add(weeklyMenuTable);
        await _context.SaveChangesAsync(); // Get generated ID

        // 7. Create weekly menu for 7 days
        var random = new Random();
        string[] daysOfWeek = { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sábado", "Domingo" };

        for (int i = 0; i < 7; i++)
        {
            // Pick 3 random distinct recipes
            var selectedRecipes = validRecipes.OrderBy(x => random.Next()).Take(3).ToList();
            var menu = new Menu
            {
                name = $"Menú del {daysOfWeek[i].ToLower()}",
                description = "Menú diseñado para apoyar tus objetivos nutricionales.",
                day_of_week = daysOfWeek[i],
                created_at = DateTime.UtcNow,
                user_id = user_id,
                Menu_Recipes = selectedRecipes.Select(r => new Menu_Recipes
                {
                    Recipe = r,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                }).ToList()
            };


            _context.Menu.Add(menu);
            await _context.SaveChangesAsync(); // Get menu ID

            var weeklyMenu = new Weekly_Menu
            {
                day_of_week = daysOfWeek[i],
                menu_id = menu.id,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                menu_table_id = weeklyMenuTable.id
            };

            _context.weekly_menu.Add(weeklyMenu);
        }

        // 8. Save all Weekly_Menu records
        await _context.SaveChangesAsync();

      return Ok(new { message = "Weekly menu generated successfully." });

    }
    catch (Exception ex)
    {
        var innerMessage = ex.InnerException?.Message;
        var innerStackTrace = ex.InnerException?.StackTrace;
        return StatusCode(500, $"Server error: {ex.Message} - {innerMessage} - StackTrace: {innerStackTrace}");
    }
}


//



    }
}
