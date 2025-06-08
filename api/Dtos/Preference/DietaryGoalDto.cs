using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Dtos.Preference
{
    public class DietaryGoalDto
    {
        public int id { get; set; }
        public string goal { get; set; }
        public DateTime created_at { get; set; }
    }
}