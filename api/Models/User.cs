using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class User
    {
        public int id { get; set; }

        [Required, EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; } // Se almacenará encriptada

        [Required]
        public string full_name { get; set; }

        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
