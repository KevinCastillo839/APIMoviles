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
            .Include(m => m.MenuRecipes)
            .ThenInclude(mr => mr.Recipe)
            .ToListAsync();

        var menusDto = menus.Select(menu => new MenuDto
        {
            id = menu.id,
            name = menu.name,
            description = menu.description,
            day_of_week = menu.day_of_week,
            menuRecipes = menu.MenuRecipes.Select(mr => new MenuRecipeDto
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

  



  }
}
