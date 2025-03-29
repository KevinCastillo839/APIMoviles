using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Recipe;
using api.Dtos.Ingredient;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  
  [Route("api/recipe")]
  [ApiController]
  [Authorize] 
  public class RecipeController : ControllerBase
  {
    private readonly ApplicationDBContext _context;

    
    public RecipeController(ApplicationDBContext context)
    {
      _context = context;
    }

    // Method to get all recipes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {

        // It retrieves all the recipes with their related ingredients.
        var recipes = await _context.Recipes
            .Include(r => r.Recipe_Ingredients)
            .ThenInclude(ri => ri.Ingredient) 
            .ToListAsync();

        // It maps the recipes to a format more suitable for the response.

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
        }).ToList();

        // It returns the list of mapped recipes.
        return Ok(recipesDto);
    }

    // Method to get a recipe by its ID.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Searches for a specific recipe along with its related ingredients.
        var recipe = await _context.Recipes
            .Include(r => r.Recipe_Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.id == id);

        // If the recipe is not found, it returns an error message.
        if (recipe == null)
        {
            return NotFound(new { message = "Receta no encontrada" });
        }

        // Maps the retrieved recipe to a suitable format for the response.
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

        //Returns the requested recipe.
        return Ok(recipeDto);
    }

    //Method to create a new recipe.
    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeRequestDto request)
    {
        // Checks if the model is valid (the recipe data).
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // If the model is not valid, it returns the validation errors.
        }

        // It creates a new recipe object with the data provided in the request.
        var recipe = new Recipe
        {
            name = request.name,
            instructions = request.instructions,
            category = request.category,
            preparation_time = request.preparation_time,
            image_url = request.image_url,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        // It adds the recipe to the context and saves the changes to the database.
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        // It maps the recipe ingredients and adds them to the database.
        var recipeIngredients = request.Recipe_Ingredients.Select(ri => new Recipe_Ingredient
        {
            recipe_id = recipe.id,
            ingredient_id = ri.ingredient_id,
            quantity = ri.quantity,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        }).ToList();

        _context.Recipe_Ingredients.AddRange(recipeIngredients);
        await _context.SaveChangesAsync();

        // It returns a "Created" status with the URL to access the newly created recipe.
        return CreatedAtAction(nameof(GetById), new { id = recipe.id }, recipe);
    }

    // Method to update an existing recipe
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(int id, [FromBody] UpdateRecipeRequestDto request)
    {
        // Verifies if the model is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // If it is not valid, return the validation errors.
        }

        // Searches for the recipe in the database
        var recipe = await _context.Recipes.Include(r => r.Recipe_Ingredients).FirstOrDefaultAsync(r => r.id == id);
        if (recipe == null)
        {
            return NotFound(new { message = "Receta no encontrada" }); // If it does not exist, return an error message.
        }

        // Updates the recipe data
        recipe.name = request.name;
        recipe.instructions = request.instructions;
        recipe.category = request.category;
        recipe.preparation_time = request.preparation_time;
        recipe.image_url = request.image_url;
        recipe.updated_at = DateTime.UtcNow;

        // Deletes the existing ingredients and adds the new ones
        _context.Recipe_Ingredients.RemoveRange(recipe.Recipe_Ingredients);
        
        var recipeIngredients = request.Recipe_Ingredients.Select(ri => new Recipe_Ingredient
        {
            recipe_id = recipe.id,
            ingredient_id = ri.ingredient_id,
            quantity = ri.quantity,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        }).ToList();

        _context.Recipe_Ingredients.AddRange(recipeIngredients);
        await _context.SaveChangesAsync();

        // Returns a 'No Content' status indicating that the update was successful
        return NoContent();
    }

    // Method to delete a recipe
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        // Search for the recipe in the database
        var recipe = await _context.Recipes.Include(r => r.Recipe_Ingredients).FirstOrDefaultAsync(r => r.id == id);
        if (recipe == null)
        {
            return NotFound(new { message = "Receta no encontrada" }); // If it does not exist, return an error message
        }

        // Remove the related ingredients and then the recipe
        _context.Recipe_Ingredients.RemoveRange(recipe.Recipe_Ingredients);
        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();

        // Return a "No Content" status indicating that the deletion was successful
        return NoContent();
    }
  }

}

