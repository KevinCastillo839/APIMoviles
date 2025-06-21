using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;

namespace api.Dtos.Preference
{
    public class CreateUserDietaryGoalRequestDto
    {
        public int user_preference_id { get; set; }
        public int goal_id { get; set; }   
        public DateTime created_at { get; set; }
    }

}