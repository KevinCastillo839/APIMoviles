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
using api.Dtos.Ingredient;

namespace api.Controllers
{
  [Route("api/unit_measurement")]
  [ApiController]
  //[Authorize] 
  public class UnitMeasurementController : ControllerBase
  {
    private readonly ApplicationDBContext _context;

    public UnitMeasurementController(ApplicationDBContext context)
    {
      _context = context;
    }

 [HttpGet]
public async Task<IActionResult> GetAll()
{
    try
    {
        var units = await _context.unit_measurement.ToListAsync();

        if (!units.Any())
        {
            return NotFound("No se encontraron unidades de medida.");
        }

        var result = units.Select(u => new UnitMeasurementDto
        {
            id = u.id,
            name = u.name,
            created_at = u.created_at,
            updated_at = u.updated_at
        }).ToList();

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Ocurrió un error al obtener las unidades de medida: {ex.Message}");
    }
}


  }
}