using System;
using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.Dtos.Recipe
{

    public class CreateRecipeAllergyDto
    {
       public int recipe_id { get; set; }  
        public int allergy_id { get; set; }  
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}

