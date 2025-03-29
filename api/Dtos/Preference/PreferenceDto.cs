using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Auth;

namespace api.Dtos.Preference
{
    public class PreferenceDto
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public bool is_vegetarian { get; set; }
        public bool is_gluten_free { get; set; }
        public bool is_vegan { get; set; }
        public string dietary_goals { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public UserDto User { get; set; }
        public List<UserAllergyDto> User_Allergies { get; set; } = new List<UserAllergyDto>();
        
    }
}