using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class User_Dietary_Restriction
    {
        public int id { get; set; }
        public int user_preference_id { get; set; }

        [ForeignKey("user_preference_id")]
        public Preference preference { get; set; }
        public int restriction_id { get; set; }   
        [ForeignKey("restriction_id")]
        public Dietary_Restriction dietary_Restriction { get; set; }  
    }
}