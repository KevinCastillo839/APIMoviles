using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Preference
{
    public class CreateUserAllergyRequestDto
    {
        public int user_preference_id { get; set;}
        public int allergie_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}