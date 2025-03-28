using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class User_Allergy
    {
        public int id { get; set;}
        public int user_preferences_id { get; set;}

        [ForeignKey("user_preferences_id")]
        public Preference User_Preference { get; set; }

        public int allergy_id { get; set; }
        
        [ForeignKey("allergy_id")]
        public Allergy Allergy { get; set; }

        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}