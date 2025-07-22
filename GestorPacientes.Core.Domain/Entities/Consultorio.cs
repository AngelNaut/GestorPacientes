using GestorPacientes.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Domain.Entities
{
    public class Consultorio : AuditableBaseEntity
    {
        
        public string Nombre { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
        public ICollection<Medico> Medicos { get; set; }
        public ICollection<PruebaLaboratorio> PruebasLaboratorio { get; set; }
        public ICollection<Paciente> Pacientes { get; set; }
        public ICollection<Cita> Citas { get; set; }
        public ICollection<ResultadoLaboratorio> ResultadosLaboratorio { get; set; }
    }
}
