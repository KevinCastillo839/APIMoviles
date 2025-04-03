using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Auth;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Dtos.Allergy;

namespace api.Controllers
{
  [Route("api/user_allergy")]
  [ApiController]
  [Authorize] 
  public class UserAllergyController : ControllerBase
  {
    private readonly ApplicationDBContext _context;

    public UserAllergyController(ApplicationDBContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var userAllergies = await _context.User_Allergies
                .Include(u => u.User)  
                .Include(ua => ua.Allergy) 
                .ToListAsync();

            if (!userAllergies.Any())
            {
                return NotFound("No se encontraron registros en la tabla User_Allergies.");
            }


            var userAllergyDto = userAllergies.Select(userAllergies => new UserAllergyDto
            {
                id = userAllergies.id,
                user_id=userAllergies.user_id,
                User = new UserDto // Convert User entity to DTO
                {
                    id = userAllergies.User.id,
                    full_name = userAllergies.User.full_name
                },
                allergie_id=userAllergies.allergy_id,
                Allergy = new AllergyDto 
                {
                    id = userAllergies.Allergy.id,
                    name = userAllergies.Allergy.name,
                    description=userAllergies.Allergy.description,
                }
                
            }).ToList();

            return Ok(userAllergyDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrió un error al obtener los datos: {ex.Message}");
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var userAllergies = await _context.User_Allergies
                .Where(ua => ua.user_id == userId)
                .Include(ua => ua.User)     
                .Include(ua => ua.Allergy)  
                .ToListAsync();

            if (!userAllergies.Any())
            {
                return NotFound($"No se encontraron alergias para el usuario con ID {userId}.");
            }

            var userAllergyDto = userAllergies.Select(ua => new UserAllergyDto
            {
                id = ua.id,
                user_id = ua.user_id,
                User = new UserDto 
                {
                    id = ua.User.id,
                    full_name = ua.User.full_name
                },
                allergie_id = ua.allergy_id,
                Allergy = new AllergyDto 
                {
                    id = ua.Allergy.id,
                    name = ua.Allergy.name,
                    description = ua.Allergy.description,
                }
            }).ToList();

            return Ok(userAllergyDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrió un error al obtener los datos: {ex.Message}");
        }
    }
   
    [HttpPost]
    public async Task<IActionResult> CreateUserAllergies([FromBody] CreateUserAllergyRequestDto requestDto)
    {
        try
        {
            if (requestDto.allergy_ids == null || !requestDto.allergy_ids.Any())
            {
                return BadRequest("Debe proporcionar al menos una alergia.");
            }
            var uniqueAllergyIds = requestDto.allergy_ids.Distinct().ToList();

            var userAllergies = uniqueAllergyIds.Select(allergyId => new User_Allergy
            {
                user_id = requestDto.user_id,
                allergy_id = allergyId,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            }).ToList();

            _context.User_Allergies.AddRange(userAllergies);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Alergias guardadas correctamente.", userAllergies });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al guardar las alergias del usuario: {ex.Message}");
        }
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUserAllergies(int userId, [FromBody] CreateUserAllergyRequestDto userAllergyDto)
    {
        try
        {
            // Get the user's current allergies
            var existingUserAllergies = await _context.User_Allergies
                .Where(ua => ua.user_id == userId)
                .ToListAsync();

            var existingAllergyIds = existingUserAllergies.Select(ua => ua.allergy_id).ToList();
            var newAllergyIds = userAllergyDto.allergy_ids;

            // Identify allergies that need to be eliminated (those that are no longer on the new list)
            var allergiesToRemove = existingUserAllergies
                .Where(ua => !newAllergyIds.Contains(ua.allergy_id))
                .ToList();

            // Identify new allergies that need to be added
            var allergiesToAdd = newAllergyIds
                .Where(id => !existingAllergyIds.Contains(id))
                .Select(allergyId => new User_Allergy
                {
                    user_id = userId,
                    allergy_id = allergyId,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                })
                .ToList();

            // Remove allergies that are no longer listed
            if (allergiesToRemove.Any())
            {
                _context.User_Allergies.RemoveRange(allergiesToRemove);
            }

            // Add new allergies
            if (allergiesToAdd.Any())
            {
                _context.User_Allergies.AddRange(allergiesToAdd);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Alergias del usuario actualizadas correctamente.",
                removed = allergiesToRemove.Select(a => a.allergy_id).ToList(),
                added = allergiesToAdd.Select(a => a.allergy_id).ToList()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar las alergias del usuario: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserAllergy(int id)
    {
        try
        {
            var userAllergy = await _context.User_Allergies.FindAsync(id);
            if (userAllergy == null)
            {
                return NotFound($"No se encontró la relación con ID {id}.");
            }

            _context.User_Allergies.Remove(userAllergy);
            await _context.SaveChangesAsync();

            return Ok($"La alergia del usuario con ID {id} ha sido eliminada.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar la alergia del usuario: {ex.Message}");
        }
    }


  }
}