using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Auth;
using api.Dtos.Preference;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/dietary_goal")]
    [ApiController]

    public class DietaryGoalController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public DietaryGoalController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var dietaryGoal = await _context.Dietary_Goals.ToListAsync();
                var dietaryGoalDto = dietaryGoal.Select(dietaryGoal => dietaryGoal.ToDto());
                return Ok(dietaryGoalDto);
            }
            catch (Exception ex)
            {
                // Return a 500 status code with error details in case of an exception
                return StatusCode(500, $"Ocurrió un error al obtener los objetivos de dieta: {ex.Message}");
            }
        }
[HttpPost]
public async Task<IActionResult> AddDietaryGoals([FromBody] CreateUserDietaryGoalRequestDto request)
{
    if (request == null || request.user_preference_id <= 0 || request.goal_id <= 0)
    {
        return BadRequest(new { error = "Datos inválidos en la solicitud." });
    }

    var preference = await _context.user_preferences.FindAsync(request.user_preference_id);
    if (preference == null)
    {
        return NotFound(new { error = "Preferencia no encontrada." });
    }

    var dietaryGoal = new User_Dietary_Goal
    {
        user_preference_id = request.user_preference_id,
        goal_id = request.goal_id,
    };

    try
    {
        _context.User_Dietary_Goals.Add(dietaryGoal);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Objetivo agregado exitosamente." });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = $"Error al agregar objetivos: {ex.Message}" });
    }
}

    }
}