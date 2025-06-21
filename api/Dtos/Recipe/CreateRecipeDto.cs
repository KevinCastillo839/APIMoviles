using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Recipe;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace api.Dtos.Ingredient
{
  public class CreateRecipeRequestDto
  {

    public string name { get; set; }
    public string instructions { get; set; }
    public string category { get; set; }
    public int preparation_time { get; set; }
    public IFormFile? image { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow; // Evita valores fuera de rango
    public DateTime? updated_at { get; set; } = null;

    [Required]
    public string Recipe_IngredientsJson { get; set; }
         public int? user_id { get; set; }
      
  }
}