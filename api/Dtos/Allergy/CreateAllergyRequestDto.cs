using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Allergy
{
  public class CreateAllergyRequestDto
  {

        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
  }
}