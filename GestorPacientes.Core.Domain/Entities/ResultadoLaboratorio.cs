using GestorPacientes.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Domain.Entities
{
    public class ResultadoLaboratorio : AuditableBaseEntity
    {
       
        public string Resultado { get; set; }
        public string Estado { get; set; } // "Pendiente", "Completada"

        // Relaciones
        public int CitaId { get; set; }
        public Cita Cita { get; set; }

        public int PruebaLaboratorioId { get; set; }
        public PruebaLaboratorio PruebaLaboratorio { get; set; }

        public int ConsultorioId { get; set; }
        public Consultorio Consultorio { get; set; }

    }
}
