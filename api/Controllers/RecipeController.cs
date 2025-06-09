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
using System.Text.Json;
using System.ComponentModel.DataAnnotations;



namespace api.Controllers
{
  
  [Route("api/recipe")]
  [ApiController]

  public class RecipeController : ControllerBase
  {
    private readonly ApplicationDBContext _context;
    private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages");
    
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
                    recipe_id = ri.RecipeId, //recipe_id
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
                    recipe_id = ri.RecipeId, //recipe_id
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
public async Task<IActionResult> CreateRecipe([FromForm] CreateRecipeRequestDto request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    // Crear receta base
    var recipe = new Recipe
    {
        name = request.name,
        instructions = request.instructions,
        category = request.category,
        preparation_time = request.preparation_time,
        created_at = request.created_at,
        updated_at = request.updated_at
    };

    await _context.Recipes.AddAsync(recipe);
    await _context.SaveChangesAsync();

    // Guardar imagen
    if (request.image != null && request.image.Length > 0)
    {
        var fileName = recipe.id + Path.GetExtension(request.image.FileName);
        var filePath = Path.Combine(_imagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.image.CopyToAsync(stream);
        }

        recipe.image_url = fileName;
        _context.Recipes.Update(recipe);
        await _context.SaveChangesAsync();
    }

    // Parsear ingredientes
    var recipeIngredients = JsonSerializer.Deserialize<List<CreateRecipeIngredientDto>>(request.Recipe_IngredientsJson);

    foreach (var item in recipeIngredients)
    {
        item.recipe_id = recipe.id;

        // Validar campos requeridos en cada ingrediente
        var context = new ValidationContext(item);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(item, context, results, true))
        {
            return BadRequest(results);
        }
    }

    var mappedIngredients = recipeIngredients.Select(ri => new Recipe_Ingredient
    {
        RecipeId = recipe.id,
        ingredient_id = ri.ingredient_id,
        quantity = ri.quantity,
        created_at = ri.created_at,
        updated_at = ri.updated_at
    }).ToList();

    _context.Recipe_Ingredients.AddRange(mappedIngredients);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetById), new { id = recipe.id }, recipe);
}

[HttpPut("{id}")]
public async Task<IActionResult> UpdateRecipe(int id, [FromForm] UpdateRecipeRequestDto request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var recipe = await _context.Recipes
        .Include(r => r.Recipe_Ingredients)
        .FirstOrDefaultAsync(r => r.id == id);

    if (recipe == null)
        return NotFound(new { message = "Receta no encontrada" });

    // Actualizar campos de la receta
    recipe.name = request.name;
    recipe.instructions = request.instructions;
    recipe.category = request.category;
    recipe.preparation_time = request.preparation_time;
    recipe.updated_at = DateTime.UtcNow;

    // Actualizar imagen si hay archivo nuevo
    if (request.image != null && request.image.Length > 0)
    {
        var fileName = recipe.id + Path.GetExtension(request.image.FileName);
        var filePath = Path.Combine(_imagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.image.CopyToAsync(stream);
        }

        recipe.image_url = fileName;
    }

    // Solo actualizar ingredientes si se envió algo en Recipe_IngredientsJson
    if (!string.IsNullOrWhiteSpace(request.Recipe_IngredientsJson))
    {
        List<RecipeIngredientDto> ingredientDtos;

        try
        {
            ingredientDtos = System.Text.Json.JsonSerializer.Deserialize<List<RecipeIngredientDto>>(request.Recipe_IngredientsJson);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "JSON de ingredientes inválido", error = ex.Message });
        }

        // Validar cada ingrediente
        foreach (var ingredientDto in ingredientDtos)
        {
            var context = new ValidationContext(ingredientDto);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(ingredientDto, context, results, true))
                return BadRequest(results);
        }

        // Remover ingredientes antiguos
        _context.Recipe_Ingredients.RemoveRange(recipe.Recipe_Ingredients);

        // Agregar ingredientes nuevos
        var mappedIngredients = ingredientDtos.Select(ri => new Recipe_Ingredient
        {
            RecipeId = recipe.id,
            ingredient_id = ri.ingredient_id,
            quantity = ri.quantity,
            unit_measurement_id = ri.unit_measurement_id, // se agregó
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        }).ToList();

        _context.Recipe_Ingredients.AddRange(mappedIngredients);
    }

    await _context.SaveChangesAsync();

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
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetRecipesForUser(int userId)
    {
        var recipes = await _context.Recipes
            .FromSqlInterpolated($"EXEC GetRecipesExcludingUserAllergies @UserId = {userId}")
            .ToListAsync();

        // Gets the IDs of the returned recipes
        var recipeIds = recipes.Select(r => r.id).ToList();

        // Check the ingredients related to those recipes
        var recipeIngredients = await _context.Recipe_Ingredients
            .Where(ri => recipeIds.Contains(ri.RecipeId))
            .Include(ri => ri.Ingredient)
            .ToListAsync();

        // Map recipes with their ingredients to the DTO
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
            Recipe_Ingredients = recipeIngredients
                .Where(ri => ri.RecipeId == recipe.id)
                .Select(ri => new RecipeIngredientDto
                {
                    id = ri.id,
                    recipe_id = ri.RecipeId,
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

        // Returns recipes with their ingredients
        return Ok(recipesDto);
    }


  }
  

}
