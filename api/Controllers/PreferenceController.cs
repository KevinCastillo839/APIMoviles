using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
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
            var user_preferences = await _context.user_preferences.ToListAsync();
            var preferencesDto = user_preferences.Select(user_preferences => user_preferences.ToDto());
            return Ok(preferencesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var preference = await _context.user_preferences.FirstOrDefaultAsync(p => p.id == id);
            if (preference == null)
            {
                return NotFound();
            }
            return Ok(preference.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePreferenceRequestDto preferenceDto)
        {
            var preferenceModel = preferenceDto.ToPreferenceFromCreateDto();
            await _context.user_preferences.AddAsync(preferenceModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = preferenceModel.id }, preferenceModel.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePreferenceRequestDto preferenceDto)
        {
            var preferenceModel = await _context.user_preferences.FirstOrDefaultAsync(p => p.id == id);
            if (preferenceModel == null)
            {
                return NotFound();
            }

            preferenceModel.user_id = preferenceDto.user_id;
            preferenceModel.is_vegetarian = preferenceDto.is_vegetarian;
            preferenceModel.is_gluten_free = preferenceDto.is_gluten_free;
            preferenceModel.is_vegan = preferenceDto.is_vegan;
            preferenceModel.dietary_goals = preferenceDto.dietary_goals;
            preferenceModel.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(preferenceModel.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var preferenceModel = await _context.user_preferences.FirstOrDefaultAsync(p => p.id == id);
            if (preferenceModel == null)
            {
                return NotFound();
            }

            _context.user_preferences.Remove(preferenceModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
