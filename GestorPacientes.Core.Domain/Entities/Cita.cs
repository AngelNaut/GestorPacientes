using GestorPacientes.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Domain.Entities
{
    public class Cita : AuditableBaseEntity
    {
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraCita { get; set; }
        public string Causa { get; set; }
        public string Estado { get; set; } // "PendienteConsulta", "PendienteResultados", "Completada"

        // Relaciones
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        public int MedicoId { get; set; }
        public Medico Medico { get; set; }

        public int ConsultorioId { get; set; }
        public Consultorio Consultorio { get; set; }

        public ICollection<ResultadoLaboratorio> ResultadosLaboratorio { get; set; } // 👈 Agregado
    }
}
