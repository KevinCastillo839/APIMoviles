using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Recipe_Ingredient
    {
        public int id { get; set; }
        public int recipe_id { get; set; }  // Relación con Recipe

        [ForeignKey("recipe_id")]
        public Recipe Recipe { get; set; }  // Relación con Recipe

        public int ingredient_id { get; set; }  // Clave foránea que hace referencia a Ingredient

        [ForeignKey("ingredient_id")]
        public Ingredient Ingredient { get; set; }  // Relación con Ingredient

        public string quantity { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
