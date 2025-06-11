using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class User_Dietary_Goal
    {
        public int id { get; set; }
        public int user_preference_id { get; set; }

        [ForeignKey("user_preference_id")]
        public Preference preference { get; set; }
        public int goal_id { get; set; }   
        [ForeignKey("goal_id")]
        public Dietary_Goal dietary_Goal { get; set; }  
    }
}