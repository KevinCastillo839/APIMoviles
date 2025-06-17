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
    [Authorize] 

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
        [HttpPut]
        public async Task<IActionResult> UpdateDietaryGoal([FromBody] CreateUserDietaryGoalRequestDto request)
        {
            if (request == null || request.user_preference_id <= 0 || request.goal_id <= 0)
            {
                return BadRequest(new { error = "Datos inválidos en la solicitud." });
            }

            // Check if the preference exists
            var preference = await _context.user_preferences.FindAsync(request.user_preference_id);
            if (preference == null)
            {
                return NotFound(new { error = "Preferencia no encontrada." });
            }

            try
            {
                // Remove any existing targets for the preference
                var existingGoal = _context.User_Dietary_Goals
                    .FirstOrDefault(g => g.user_preference_id == request.user_preference_id);

                if (existingGoal != null)
                {
                    _context.User_Dietary_Goals.Remove(existingGoal);
                }

                // Create the new goal
                var newGoal = new User_Dietary_Goal
                {
                    user_preference_id = request.user_preference_id,
                    goal_id = request.goal_id
                };

                _context.User_Dietary_Goals.Add(newGoal);

                // Save changes
                await _context.SaveChangesAsync();

                return Ok(new { message = "Objetivo actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al actualizar el objetivo: {ex.Message}" });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPreferences(int userId)
        {
            try
            {
                var preferences = await _context.UserPreferences
                    .FromSqlInterpolated($"EXEC GetUserPreferences @UserId = {userId}")
                    .ToListAsync();

                var preferenceId = preferences.FirstOrDefault()?.user_preference_id;

                // Organize the results
                var restrictions = preferences
                    .Where(p => p.restriction_id != null)
                    .Select(p => new
                    {
                        id = p.restriction_id,
                        name = p.restriction
                    })
                    .ToList();

                var goal = preferences
                    .Where(p => p.goal_id != null)
                    .Select(p => new
                    {
                        id = p.goal_id,
                        goal = p.goal
                    })
                    .FirstOrDefault(); // Just one goal

                // Structure of the result
                var result = new
                {
                    UserId = userId,
                    PreferenceId = preferenceId,
                    Restrictions = restrictions,
                    Goal = goal
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al obtener las preferencias del usuario: {ex.Message}");
            }
        }

    }
}