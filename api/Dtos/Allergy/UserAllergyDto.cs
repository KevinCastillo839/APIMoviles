using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Auth;

namespace api.Dtos.Allergy
{
    public class UserAllergyDto
    {
        public int id { get; set;}
        public int user_id { get; set;}
        public int allergie_id { get; set; }
        public AllergyDto Allergy { get; set; } 
        public UserDto User { get; set; }
    }
}