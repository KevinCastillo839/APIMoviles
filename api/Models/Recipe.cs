using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Recipe
    {
        public int id { get; set; }
        public string name { get; set; }
        public string instructions { get; set; }
        public string category { get; set; }
        public int preparation_time { get; set; }
        public string image_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public List<Recipe_Ingredient> Recipe_Ingredients { get; set; } = new List<Recipe_Ingredient>();

        public int? user_id { get; set; }
        public User User { get; set; }


    }
}
