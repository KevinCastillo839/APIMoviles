using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Preference
{
    public class UpdatePreferenceRequestDto
    {
        public int user_id { get; set; }
        public bool is_vegetarian { get; set; }
        public bool is_gluten_free { get; set; }
        public bool is_vegan { get; set; }
        public string dietary_goals { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        
    }

}