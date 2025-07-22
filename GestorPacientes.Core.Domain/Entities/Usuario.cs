using GestorPacientes.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Domain.Entities
{
    public class Usuario : AuditableBaseEntity
    {
        public int UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        [Required, MaxLength(100)]
        public string NombreUsuario { get; set; }
        public string? FotoRuta { get; set; }
        [Required, MaxLength(200)]        
        public string Contrasena { get; set; }
        public string TipoUsuario { get; set; } // "Administrador" o "Asistente"

        // Relación
        public int ConsultorioId { get; set; }
        public Consultorio Consultorio { get; set; }
    }
}
