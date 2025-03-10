using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Recipe;
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
      var recipes = await _context.Recipes.ToListAsync();
      var recipesDto = recipes.Select(recipes => recipes.ToDto());
      return Ok(recipesDto);
    }
  }
  
}
