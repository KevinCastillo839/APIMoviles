using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;

namespace api.Dtos.Preference
{
    public class CreateUserDietaryRestrictionRequestDto
    {
       public int user_preference_id { get; set; }
       public List<int> restriction_ids { get; set; } = new List<int>();
    }

}