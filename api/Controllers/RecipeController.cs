using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Recipe;
using api.Dtos.Ingredient;

using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  [Route("api/recipe")]
  [ApiController]
  public class RecipeController : ControllerBase
  {
    private readonly ApplicationDBContext _context;
    public RecipeController(ApplicationDBContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var recipes = await _context.Recipes
            .Include(r => r.Recipe_Ingredients)
            .ThenInclude(ri => ri.Ingredient) // Incluye los detalles del ingrediente
            .ToListAsync();

        var recipesDto = recipes.Select(recipe => new RecipeDto
        {
            id = recipe.id,
            name = recipe.name,
            instructions = recipe.instructions,
            category = recipe.category,
            preparation_time = recipe.preparation_time,
            image_url = recipe.image_url,
            created_at = recipe.created_at,
            updated_at = recipe.updated_at,
            Recipe_Ingredients = recipe.Recipe_Ingredients
                .Where(ri => ri.recipe_id == recipe.id) // Asegura la relación
                .Select(ri => new RecipeIngredientDto
                {
                    id = ri.id,
                    recipe_id = ri.recipe_id,
                    ingredient_id = ri.ingredient_id,
                    quantity = ri.quantity,
                    Ingredient = new IngredientDto
                    {
                        id = ri.Ingredient.id, // Relaciona el id del ingrediente
                        name = ri.Ingredient.name, // Puedes agregar más campos del ingrediente si lo necesitas
                        description = ri.Ingredient.description
                    }
                }).ToList()
        }).ToList();

        return Ok(recipesDto);
    } 
    [HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var recipe = await _context.Recipes
        .Include(r => r.Recipe_Ingredients)
        .ThenInclude(ri => ri.Ingredient) // Incluye los detalles del ingrediente
        .FirstOrDefaultAsync(r => r.id == id);

    if (recipe == null)
    {
        return NotFound();
    }

    var recipeDto = new RecipeDto
    {
        id = recipe.id,
        name = recipe.name,
        instructions = recipe.instructions,
        category = recipe.category,
        preparation_time = recipe.preparation_time,
        image_url = recipe.image_url,
        created_at = recipe.created_at,
        updated_at = recipe.updated_at,
        Recipe_Ingredients = recipe.Recipe_Ingredients
            .Select(ri => new RecipeIngredientDto
            {
                id = ri.id,
                recipe_id = ri.recipe_id,
                ingredient_id = ri.ingredient_id,
                quantity = ri.quantity,
                Ingredient = new IngredientDto
                {
                    id = ri.Ingredient.id,
                    name = ri.Ingredient.name,
                    description = ri.Ingredient.description
                }
            }).ToList()
    };

    return Ok(recipeDto);
}

[HttpPost]
public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeRequestDto request)
{
    if (request == null || request.Recipe_Ingredients == null || !request.Recipe_Ingredients.Any())
    {
        return BadRequest("La receta y sus ingredientes son requeridos.");
    }

        var recipe = new Recipe
        {
            name = request.name,
            instructions = request.instructions,
            category = request.category,
            preparation_time = request.preparation_time,
            image_url = request.image_url,
            created_at = DateTime.UtcNow, // Asigna la fecha actual para evitar valores nulos
            updated_at = DateTime.UtcNow
        };


    _context.Recipes.Add(recipe);
    await _context.SaveChangesAsync();

    // Inserta solo las relaciones con ingredientes existentes
var recipeIngredients = request.Recipe_Ingredients.Select(ri => new Recipe_Ingredient
{
    recipe_id = recipe.id,
    ingredient_id = ri.ingredient_id,
    quantity = ri.quantity,
    created_at = DateTime.UtcNow, // Agregar fechas
    updated_at = DateTime.UtcNow
}).ToList();


    _context.Recipe_Ingredients.AddRange(recipeIngredients);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetById), new { id = recipe.id }, recipe);
}


}
}
