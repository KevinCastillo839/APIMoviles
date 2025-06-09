using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Recipe_Ingredient
    {
        public int id { get; set; }

        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }

        [Column("ingredient_id")]
        public int ingredient_id { get; set; }

        [ForeignKey("ingredient_id")]
        public Ingredient Ingredient { get; set; }

        [Column("unit_measurement_id")]
        public int unit_measurement_id { get; set; }

        [ForeignKey("unit_measurement_id")]
        public Unit_Measurement Unit_Measurement { get; set; }

        [Column("quantity")]
        public decimal quantity { get; set; }

        [Column("created_at")]
        public DateTime created_at { get; set; }

        [Column("updated_at")]
        public DateTime updated_at { get; set; }
    }
}
