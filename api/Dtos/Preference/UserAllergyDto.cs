using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Allergy;

namespace api.Dtos.Preference
{
    public class UserAllergyDto
    {
        public int id { get; set;}
        public int user_id { get; set;}
        public int allergie_id { get; set; }
        public List<AllergyDto> allergies { get; set; } = new List<AllergyDto>();
    }
}