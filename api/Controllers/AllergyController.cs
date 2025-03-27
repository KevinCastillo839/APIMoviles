using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Allergy; // Add this using for the DTOs
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  [Route("api/allergy")]
  [ApiController]
  [Authorize] 
  public class AllergyController : ControllerBase
  {
    private readonly ApplicationDBContext _context;

    public AllergyController(ApplicationDBContext context)
    {
      _context = context;
    }

    // Get all allergies
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var allergies = await _context.Allergies.ToListAsync();
      var allergiesDto = allergies.Select(allergy => allergy.ToDto());
      return Ok(allergiesDto);
    }

    // Get allergy by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
      var allergy = await _context.Allergies.FirstOrDefaultAsync(u => u.id == id);
      if (allergy == null)
      {
        return NotFound(new { message = "Alergia no encontrada" });
      }
      return Ok(allergy.ToDto());
    }

    //Create a new allergy
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAllergyRequestDto eventDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        // Check if an allergy with the same name already exists
        bool exists = await _context.Allergies.AnyAsync(a => a.name == eventDto.name);
        if (exists)
        {
          return Conflict(new { message = "Ya existe una alergia con este nombre" });
        }

        var allergyModel = eventDto.ToAllergyFromCreateDto();
        allergyModel.created_at = DateTime.UtcNow; // Asignar fecha de creación
        await _context.Allergies.AddAsync(allergyModel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = allergyModel.id }, allergyModel.ToDto());
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error al crear la alergia", error = ex.Message });
      }
    }

    // Update allergy by ID
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAllergyRequestDto allergyDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var allergyModel = await _context.Allergies.FirstOrDefaultAsync(a => a.id == id);
        if (allergyModel == null)
        {
          return NotFound(new { message = "Alergia no encontrada" });
        }

        //Update the data
        allergyModel.name = allergyDto.name;
        allergyModel.description = allergyDto.description;
        allergyModel.updated_at = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(allergyModel.ToDto());
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error al actualizar la alergia", error = ex.Message });
      }
    }

    // Delete allergy by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
      try
      {
        var allergyModel = await _context.Allergies.FirstOrDefaultAsync(a => a.id == id);
        if (allergyModel == null)
        {
          return NotFound(new { message = "Alergia no encontrada" });
        }

        _context.Allergies.Remove(allergyModel);
        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error al eliminar la alergia", error = ex.Message });
      }
    }
  }
}
