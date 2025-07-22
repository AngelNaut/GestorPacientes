using GestorPacientes.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Domain.Entities
{
    public class Paciente : AuditableBaseEntity
    {
       
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool EsFumador { get; set; }
        public bool TieneAlergias { get; set; }
        public string? Foto { get; set; }

        // Relación
        public int ConsultorioId { get; set; }
        public Consultorio Consultorio { get; set; }

        public ICollection<Cita> Citas { get; set; }
    }
}
