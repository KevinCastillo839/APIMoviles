using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Allergy
{
    public class CreateAllergyRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ,.:\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string name { get; set; } = string.Empty;
       
        [Required(ErrorMessage = "La descripcion es obligatoria.")]
        [StringLength(255, ErrorMessage = "La descripción no puede tener más de 255 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ,.:\s]+$" , ErrorMessage = "La descripción solo puede contener letras y espacios.")]
        public string? description { get; set; }

        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
