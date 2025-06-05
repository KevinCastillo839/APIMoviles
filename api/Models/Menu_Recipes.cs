using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace Api.Models
{
    public class Menu_Recipes
    {
        public int id { get; set; }
        public int menu_id { get; set; }
        public int recipe_id { get; set; }
        public string meal_time { get; set; } = string.Empty;
        public Menu Menu { get; set; }

    // Relación con Recipe
        public Recipe Recipe { get; set; }


        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
