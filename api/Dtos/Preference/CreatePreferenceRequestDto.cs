using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;

namespace api.Dtos.Preference
{
    public class CreatePreferenceRequestDto
    {
        public int user_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

}