using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Allergy
{
    public class CreateUserAllergyRequestDto
    {
        public int user_id { get; set;}
        public List<int> allergy_ids { get; set; } = new List<int>();
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}