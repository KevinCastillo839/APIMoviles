using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Preference
    {
        public int id { get; set; }
        public int user_id { get; set; }

        [ForeignKey("user_id")]
        public User User { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
         public ICollection<User_Dietary_Goal> User_Dietary_Goals { get; set; }
        public ICollection<User_Dietary_Restriction> User_Dietary_Restrictions { get; set; }
    }
}