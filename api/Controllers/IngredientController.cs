using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Ingredient;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  [Route("api/ingredient")]
  [ApiController]
  [Authorize] 
  public class IngredientController : ControllerBase
  {
    private readonly ApplicationDBContext _context;
    public IngredientController(ApplicationDBContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var ingredients = await _context.Ingredients.ToListAsync();
      var ingredientsDto = ingredients.Select(ingredients => ingredients.ToDto());
      return Ok(ingredientsDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> getById([FromRoute] int id)
    {
      var _ingredient = await _context.Ingredients.FirstOrDefaultAsync(u => u.id == id);
      if (_ingredient == null)
      {
        return NotFound();
      }
      return Ok(_ingredient.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIngredientRequestDto eventDto)
    {
      var ingredientModel = eventDto.ToIngredientFromCreateDto();
      await _context.Ingredients.AddAsync(ingredientModel);
      await _context.SaveChangesAsync();
      return CreatedAtAction(nameof(getById), new { id = ingredientModel.id }, ingredientModel.ToDto());
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateIngredientRequestDto ingredientDto)
    {
      var ingredientModel = await _context.Ingredients.FirstOrDefaultAsync(_ingredient => _ingredient.id == id);
      if (ingredientModel == null)
      {
        return NotFound();
      }
      ingredientModel.name = ingredientDto.name;
      ingredientModel.description = ingredientDto.description;
      ingredientModel.updated_at = DateTime.UtcNow;    

      await _context.SaveChangesAsync();

      return Ok(ingredientModel.ToDto());
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
      var ingredientModel = await _context.Ingredients.FirstOrDefaultAsync(_ingredient => _ingredient.id == id);
      if (ingredientModel == null)
      {
        return NotFound();
      }
      _context.Ingredients.Remove(ingredientModel);

      await _context.SaveChangesAsync();

      return NoContent();
    }
  }
  
}
