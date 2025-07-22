using GestorPacientes.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Domain.Entities
{
    public class Medico : AuditableBaseEntity
    {
     
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }

        [Required, MaxLength(20)]
        public string Cedula { get; set; }
        public string? Foto { get; set; }

        // Relación
        public int ConsultorioId { get; set; }
        public Consultorio Consultorio { get; set; }
        public ICollection<Cita> Citas { get; set; }
    }
}
