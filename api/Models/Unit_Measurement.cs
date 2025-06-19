using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace api.Models
{
    public class Unit_Measurement
    {
          [Key]
        [Column("id")]
        public int id { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string name { get; set; }

        [Column("created_at")]
        public DateTime? created_at { get; set; }

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }
    }
}
