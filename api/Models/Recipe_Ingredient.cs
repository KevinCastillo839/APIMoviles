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

           // Nueva propiedad para la relación con Unit_Measurement
        public int unit_measurement_id { get; set; }  // Clave foránea que hace referencia a Unit_Measurement

        [ForeignKey("unit_measurement_id")]
        public Unit_Measurement Unit_Measurement { get; set; }  // Relación con Unit_Measurement

        public decimal quantity { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
