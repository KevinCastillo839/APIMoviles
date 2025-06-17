using api.Data;
using api.Dtos.Preference;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    [Route("api/dietary_restriction")]
    [ApiController]
    [Authorize] 

    public class DietaryRestrictionController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public DietaryRestrictionController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var dietaryRestriction = await _context.Dietary_Restrictions.ToListAsync();
                var dietaryRestrictionDto = dietaryRestriction.Select(dietaryRestriction => dietaryRestriction.ToDto());
                return Ok(dietaryRestrictionDto);
            }
            catch (Exception ex)
            {
                // Return a 500 status code with error details in case of an exception
                return StatusCode(500, $"Ocurrió un error al obtener las restricciones de dieta: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddDietaryRestrictions([FromBody] CreateUserDietaryRestrictionRequestDto request)
        {
            if (request == null || request.user_preference_id <= 0 || !request.restriction_ids.Any())
            {
                return BadRequest(new { message = "Datos inválidos en la solicitud." });
            }

            var preference = await _context.user_preferences.FindAsync(request.user_preference_id);
            if (preference == null)
            {
                return NotFound(new { message = "Preferencia no encontrada." });
            }

            var restrictions = request.restriction_ids.Select(id => new User_Dietary_Restriction
            {
                user_preference_id = request.user_preference_id,
                restriction_id = id
            });

            try
            {
                await _context.User_Dietary_Restrictions.AddRangeAsync(restrictions);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Restricciones agregadas exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al agregar restricciones: {ex.Message}" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDietaryRestrictions([FromBody] CreateUserDietaryRestrictionRequestDto request)
        {
            if (request == null || request.user_preference_id <= 0 || !request.restriction_ids.Any())
            {
                return BadRequest(new { message = "Datos inválidos en la solicitud." });
            }

            // Check if the preference exists
            var preference = await _context.user_preferences.FindAsync(request.user_preference_id);
            if (preference == null)
            {
                return NotFound(new { message = "Preferencia no encontrada." });
            }

            try
            {
                // Get the current restrictions for the preference
                var existingRestrictions = _context.User_Dietary_Restrictions
                    .Where(r => r.user_preference_id == request.user_preference_id);

                // Remove existing restrictions
                _context.User_Dietary_Restrictions.RemoveRange(existingRestrictions);

                // Create the new restrictions
                var newRestrictions = request.restriction_ids.Select(id => new User_Dietary_Restriction
                {
                    user_preference_id = request.user_preference_id,
                    restriction_id = id
                });

                // Add the new restrictions
                await _context.User_Dietary_Restrictions.AddRangeAsync(newRestrictions);

                // Save changes
                await _context.SaveChangesAsync();

                return Ok(new { message = "Restricciones actualizadas exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al actualizar restricciones: {ex.Message}" });
            }
        }

    }
}