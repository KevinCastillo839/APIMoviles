using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Allergy;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  [Route("api/allergy")]
  [ApiController]
 // [Authorize] 
  public class AllergyController : ControllerBase
  {
    private readonly ApplicationDBContext _context;
    public AllergyController(ApplicationDBContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var allergies = await _context.Allergies.ToListAsync();
      var allergiesDto = allergies.Select(allergies => allergies.ToDto());
      return Ok(allergiesDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> getById([FromRoute] int id)
    {
      var _allergy = await _context.Allergies.FirstOrDefaultAsync(u => u.id == id);
      if (_allergy == null)
      {
        return NotFound();
      }
      return Ok(_allergy.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAllergyRequestDto eventDto)
    {
      var allergyModel = eventDto.ToAllergyFromCreateDto();
      await _context.Allergies.AddAsync(allergyModel);
      await _context.SaveChangesAsync();
      return CreatedAtAction(nameof(getById), new { id = allergyModel.id }, allergyModel.ToDto());
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAllergyRequestDto allergyDto)
    {
      var allergyModel = await _context.Allergies.FirstOrDefaultAsync(_allergy => _allergy.id == id);
      if (allergyModel == null)
      {
        return NotFound();
      }
      
      allergyModel.name = allergyDto.name;
      allergyModel.description = allergyDto.description;
      allergyModel.updated_at = DateTime.UtcNow;    

      await _context.SaveChangesAsync();

      return Ok(allergyModel.ToDto());
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
      var allergyModel = await _context.Allergies.FirstOrDefaultAsync(_allergy => _allergy.id == id);
      if (allergyModel == null)
      {
        return NotFound();
      }
      _context.Allergies.Remove(allergyModel);

      await _context.SaveChangesAsync();

      return NoContent();
    }
  }
  
}
