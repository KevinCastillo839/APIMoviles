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
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/preference")]
    [ApiController]
    public class PreferenceController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public PreferenceController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var preference = await _context.user_preferences
                .Include(p => p.User) // Incluye la relación con el Usuario
                .Include(p => p.User_Allergies)
                .ThenInclude(pa=>pa.Allergy)
                .ToListAsync();
            
            var preferenceDto = preference.Select(preferences => new PreferenceDto
            {
                id = preferences.id,
                user_id = preferences.user_id,
                is_vegan = preferences.is_vegan,
                is_gluten_free = preferences.is_gluten_free,
                is_vegetarian = preferences.is_vegetarian,
                dietary_goals = preferences.dietary_goals,
                created_at = preferences.created_at,
                updated_at = preferences.updated_at,
                User = new UserDto // Convierte la entidad User a un DTO
                {
                    id = preferences.User.id,
                    full_name=preferences.User.full_name
                    // Agrega otros campos del usuario según sea necesario
                },
                User_Allergies = preferences.User_Allergies
                     .Where(pa=> pa.user_preferences_id == preferences.id)
                     .Select(pa=> new UserAllergyDto
                     {
                        id = pa.id,
                        user_preferences_id = pa.user_preferences_id,
                        allergie_id = pa.allergy_id,
                        Allergy = new Dtos.Allergy.AllergyDto
                        {
                            id = pa.Allergy.id,
                            name = pa.Allergy.name,
                            description = pa.Allergy.description
                        }
                     }).ToList()
            }).ToList();

             return Ok(preferenceDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            // Cargar las preferencias junto con las alergias relacionadas y la información de las alergias
            var preference = await _context.user_preferences
                .Include(p => p.User_Allergies)
                .ThenInclude(pa => pa.Allergy)
                .FirstOrDefaultAsync(p => p.id == id);

            if (preference == null)
            {
                return NotFound();
            }

            // Mapear el modelo a un DTO
            var preferenceDto = new PreferenceDto
            {
                id = preference.id,
                user_id = preference.user_id,
                is_vegan = preference.is_vegan,
                is_gluten_free = preference.is_gluten_free,
                is_vegetarian = preference.is_vegetarian,
                dietary_goals = preference.dietary_goals,
                created_at = preference.created_at,
                updated_at = preference.updated_at,
                User_Allergies = preference.User_Allergies
                    .Select(pa => new UserAllergyDto
                    {
                        id = pa.id,
                        user_preferences_id = pa.user_preferences_id,
                        allergie_id = pa.allergy_id,
                        Allergy = new Dtos.Allergy.AllergyDto
                        {
                            id = pa.Allergy.id,
                            name = pa.Allergy.name,
                            description = pa.Allergy.description
                        }
                    }).ToList()
            };

            return Ok(preferenceDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePreferenceRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Faltan datos en la solicitud.");
            }

            var preference = new Preference
            {
                user_id = request.user_id,
                is_vegetarian = request.is_vegetarian,
                is_gluten_free = request.is_gluten_free,
                is_vegan = request.is_vegan,
                dietary_goals = request.dietary_goals,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            _context.user_preferences.Add(preference);
            await _context.SaveChangesAsync();
        // Agregar alergias si existen en la solicitud
            if (request.UserAllergy != null && request.UserAllergy.Any())
            {
                var userAllergies = request.UserAllergy.Select(pa => new User_Allergy
                {
                    user_preferences_id = preference.id,
                    allergy_id = pa.allergie_id,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                }).ToList();

                _context.User_Allergies.AddRange(userAllergies);
                await _context.SaveChangesAsync();

            }

            
            return CreatedAtAction(nameof(GetById), new {id = preference.id}, preference);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePreferenceRequestDto preferenceDto)
        {
            if (preferenceDto == null)
            {
                return BadRequest("Faltan datos en la solicitud.");
            }

            // Buscar las preferencias con sus alergias relacionadas
            var preferenceModel = await _context.user_preferences
                .Include(p => p.User_Allergies)
                .FirstOrDefaultAsync(p => p.id == id);

            if (preferenceModel == null)
            {
                return NotFound("No se encontró la preferencia especificada.");
            }

            // Actualizar los datos principales de las preferencias
            preferenceModel.user_id = preferenceDto.user_id;
            preferenceModel.is_vegetarian = preferenceDto.is_vegetarian;
            preferenceModel.is_gluten_free = preferenceDto.is_gluten_free;
            preferenceModel.is_vegan = preferenceDto.is_vegan;
            preferenceModel.dietary_goals = preferenceDto.dietary_goals;
            preferenceModel.updated_at = DateTime.UtcNow;

            // Manejar las alergias asociadas
            if (preferenceDto.UserAllergy != null)
            {
                // Eliminar las alergias actuales asociadas a la preferencia
                var existingAllergies = _context.User_Allergies
                    .Where(ua => ua.user_preferences_id == preferenceModel.id);

                _context.User_Allergies.RemoveRange(existingAllergies);

                // Agregar las nuevas alergias
                var newAllergies = preferenceDto.UserAllergy.Select(pa => new User_Allergy
                {
                    user_preferences_id = preferenceModel.id,
                    allergy_id = pa.allergie_id,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                }).ToList();

                _context.User_Allergies.AddRange(newAllergies);
            }

            // Guardar todos los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Retornar la respuesta actualizada en formato DTO
            return Ok(preferenceModel.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // Cargar la preferencia junto con las alergias relacionadas
            var preferenceModel = await _context.user_preferences
                .Include(p => p.User_Allergies)
                .FirstOrDefaultAsync(p => p.id == id);

            if (preferenceModel == null)
            {
                return NotFound();
            }

            // Iniciar una transacción para garantizar la atomicidad
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Eliminar las alergias asociadas
                if (preferenceModel.User_Allergies != null && preferenceModel.User_Allergies.Any())
                {
                    _context.User_Allergies.RemoveRange(preferenceModel.User_Allergies);
                }

                // Eliminar la preferencia
                _context.user_preferences.Remove(preferenceModel);

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Confirmar la transacción
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Revertir la transacción en caso de error
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al eliminar la preferencia: {ex.Message}");
            }

            return NoContent();
        }

    }
}
