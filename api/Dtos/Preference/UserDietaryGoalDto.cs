using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Dtos.Preference
{
    public class UserDietaryGoalDto
    {
        public int id { get; set; }
        public int user_preference_id { get; set; }
        public PreferenceDto preference { get; set; }
        public int goal_id { get; set; }   
        public DietaryGoalDto dietary_Goal { get; set; }  
    }
}