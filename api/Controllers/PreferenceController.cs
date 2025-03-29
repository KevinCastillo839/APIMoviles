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
    [Route("api/preference")]
    [ApiController]
    [Authorize] 
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
            try
            {
                // Retrieve preferences including related entities
                var preferences = await _context.user_preferences
                    .Include(p => p.User) // Include the relationship with User
                    .Include(p => p.User_Allergies)
                    .ThenInclude(pa => pa.Allergy)
                    .ToListAsync();

                if (preferences == null || !preferences.Any())
                {
                    return NotFound("No se encontraron preferencias en la base de datos.");
                }

                // Map preferences to DTOs
                var preferenceDto = preferences.Select(preferences => new PreferenceDto
                {
                    id = preferences.id,
                    user_id = preferences.user_id,
                    is_vegan = preferences.is_vegan,
                    is_gluten_free = preferences.is_gluten_free,
                    is_vegetarian = preferences.is_vegetarian,
                    dietary_goals = preferences.dietary_goals,
                    created_at = preferences.created_at,
                    updated_at = preferences.updated_at,
                    User = new UserDto // Convert User entity to DTO
                    {
                        id = preferences.User.id,
                        full_name = preferences.User.full_name
                    },
                    User_Allergies = preferences.User_Allergies
                        .Where(pa => pa.user_preferences_id == preferences.id)
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
                }).ToList();

                return Ok(preferenceDto);
            }
            catch (Exception ex)
            {
                // Return a 500 status code with error details in case of an exception
                return StatusCode(500, $"Ocurrió un error al obtener las preferencias: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID proporcionado no es válido.");
            }

            try
            {
                // Load preferences along with related allergies and allergy details
                var preference = await _context.user_preferences
                    .Include(p => p.User) // Include the relationship with User
                    .Include(p => p.User_Allergies)
                    .ThenInclude(pa => pa.Allergy)
                    .FirstOrDefaultAsync(p => p.id == id);

                if (preference == null)
                {
                    return NotFound("No se encontró la preferencia solicitada.");
                }

                // Map the model to a DTO
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
                    User = new UserDto // Convert User entity to DTO
                    {
                        id = preference.User.id,
                        full_name = preference.User.full_name
                    },
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
            catch (Exception ex)
            {
                // Return a 500 status code with error details in case of an exception
                return StatusCode(500, $"Ocurrió un error al obtener la preferencia: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePreferenceRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Faltan datos en la solicitud.");
            }
            // Validate required fields
            if (request.user_id <= 0)
            {
                return BadRequest("El ID del usuario es obligatorio y debe ser válido.");
            }

            // Check if the user already has a preference
            var existingPreference = await _context.user_preferences
                .FirstOrDefaultAsync(p => p.user_id == request.user_id);

            if (existingPreference != null)
            {
                return BadRequest("El usuario ya tiene una preferencia registrada.");
            }

            // Create the preference entity
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

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Add the preference to the database
                _context.user_preferences.Add(preference);
                await _context.SaveChangesAsync();

                // Add allergies if provided
                if (request.UserAllergy != null && request.UserAllergy.Any())
                {
                    // Remove duplicates from the list sent by the user
                    var uniqueAllergies = request.UserAllergy
                        .GroupBy(pa => pa.allergie_id) // Group by allergy_id
                        .Select(group => group.First()) // Take the first element of each group (removing duplicates)
                        .ToList();

                    // Create the list of allergies to add
                    var allergiesToAdd = uniqueAllergies
                        .Select(pa => new User_Allergy
                        {
                            user_preferences_id = preference.id,
                            allergy_id = pa.allergie_id,
                            created_at = DateTime.UtcNow,
                            updated_at = DateTime.UtcNow
                        })
                        .ToList();

                    // Add non-duplicate allergies
                    if (allergiesToAdd.Any())
                    {
                        _context.User_Allergies.AddRange(allergiesToAdd);
                        await _context.SaveChangesAsync();
                    }
                }

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Rollback in case of error
                await transaction.RollbackAsync();
                return StatusCode(500, $"Ocurrió un error al crear la preferencia: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetById), new { id = preference.id }, preference);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePreferenceRequestDto preferenceDto)
        {
            if (id <= 0)
            {
                return BadRequest("El ID proporcionado no es válido.");
            }

            if (preferenceDto == null)
            {
                return BadRequest("Faltan datos en la solicitud.");
            }

            try
            {
                // Retrieve the preference with related allergies
                var preferenceModel = await _context.user_preferences
                    .Include(p => p.User_Allergies)
                    .FirstOrDefaultAsync(p => p.id == id);

                if (preferenceModel == null)
                {
                    return NotFound("No se encontró la preferencia especificada.");
                }

                // Update preference main details
                preferenceModel.user_id = preferenceDto.user_id;
                preferenceModel.is_vegetarian = preferenceDto.is_vegetarian;
                preferenceModel.is_gluten_free = preferenceDto.is_gluten_free;
                preferenceModel.is_vegan = preferenceDto.is_vegan;
                preferenceModel.dietary_goals = preferenceDto.dietary_goals;
                preferenceModel.updated_at = DateTime.UtcNow;

                // Handle associated allergies
                if (preferenceDto.UserAllergy != null)
                {
                    // Remove existing allergies linked to the preference
                    var existingAllergies = _context.User_Allergies
                        .Where(ua => ua.user_preferences_id == preferenceModel.id);

                    _context.User_Allergies.RemoveRange(existingAllergies);

                    // Add new allergies
                    var newAllergies = preferenceDto.UserAllergy.Select(pa => new User_Allergy
                    {
                        user_preferences_id = preferenceModel.id,
                        allergy_id = pa.allergie_id,
                        created_at = DateTime.UtcNow,
                        updated_at = DateTime.UtcNow
                    }).ToList();

                    _context.User_Allergies.AddRange(newAllergies);
                }

                // Save all changes to the database
                await _context.SaveChangesAsync();

                // Return the updated preference as a DTO
                return Ok(preferenceModel.ToDto());
            }
            catch (Exception ex)
            {
                // Return a 500 status code with error details in case of an exception
                return StatusCode(500, $"Ocurrió un error al actualizar la preferencia: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID proporcionado no es válido");
            }

            // Load the preference along with related allergies
            var preferenceModel = await _context.user_preferences
                .Include(p => p.User_Allergies)
                .FirstOrDefaultAsync(p => p.id == id);

            if (preferenceModel == null)
            {
                return NotFound("La preferencia no existe o ya fue eliminada.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Remove associated allergies if any
                if (preferenceModel.User_Allergies != null && preferenceModel.User_Allergies.Any())
                {
                    _context.User_Allergies.RemoveRange(preferenceModel.User_Allergies);
                }

                // Remove the preference
                _context.user_preferences.Remove(preferenceModel);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al eliminar la preferencia: {ex.Message}");
            }

            return NoContent();
        }

    }
}
