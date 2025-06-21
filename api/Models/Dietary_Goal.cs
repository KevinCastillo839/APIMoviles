using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Dietary_Goal
    {
        public int id { get; set; }
<<<<<<< HEAD:api/Models/ShoppingList.cs
        public int recipe_id { get; set; } //Foreign key
        public Recipe Recipe { get; set; } //Navigation property
        public int user_id { get; set; }
        public int menu_id { get; set; }
=======
        public string goal { get; set; }
>>>>>>> origin/main:api/Models/Dietary_Goal.cs
        public DateTime created_at { get; set; }
    }
}