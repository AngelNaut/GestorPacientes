using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.ViewModels.Pacientes
{
    public class PacienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        // Checkboxes en la vista
        public bool EsFumador { get; set; }
        public bool TieneAlergias { get; set; }

        // Foto (requerida al crear; podrías hacerla opcional al editar)
       
        public string? Foto { get; set; }

        // Para asignar el consultorio
        public int ConsultorioId { get; set; }
    }
}
