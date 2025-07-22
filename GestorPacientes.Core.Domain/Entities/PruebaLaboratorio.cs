using GestorPacientes.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Domain.Entities
{
    public class PruebaLaboratorio : AuditableBaseEntity
    {
        
        public string Nombre { get; set; }
        public int ConsultorioId { get; set; }
        public Consultorio Consultorio { get; set; }

        // Propiedad de navegación 
        public ICollection<ResultadoLaboratorio> ResultadosLaboratorio { get; set; }
    }
}
