using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;

namespace api.Models
{
    public class Menu
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string day_of_week { get; set; } = string.Empty;
        public int user_id { get; set; }
        public List<Menu_Recipes> Menu_Recipes { get; set; } = new List<Menu_Recipes>();

    }
}
