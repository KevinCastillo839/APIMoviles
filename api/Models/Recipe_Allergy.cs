using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Recipe_Allergy
    {
        public int id { get; set; }
        public int recipe_id { get; set; }  // Relación con Recipe
        [ForeignKey("recipe_id")]
        public Recipe Recipe { get; set; }  // Relación con Recipe
        public int allergy_id { get; set; }  
        [ForeignKey("allergy_id")]
        public Allergy Allergy { get; set; }  
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
