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
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public UserDto User { get; set; }
     }
}