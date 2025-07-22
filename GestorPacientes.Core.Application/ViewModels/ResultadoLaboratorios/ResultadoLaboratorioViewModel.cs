using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.ViewModels.ResultadoLaboratorios
{
    public class ResultadoLaboratorioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El resultado es obligatorio.")]
        [StringLength(1000, ErrorMessage = "El resultado no puede superar los 500 caracteres.")]
        public string Resultado { get; set; }

        // "Pendiente" o "Completada"
        public string Estado { get; set; }

        [Required(ErrorMessage = "La cita es obligatoria.")]
        public int CitaId { get; set; }

        [Required(ErrorMessage = "La prueba de laboratorio es obligatoria.")]
        public int PruebaLaboratorioId { get; set; }

        // Asignado automáticamente
        public int ConsultorioId { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteCedula { get; set; }
        public string PruebaNombre { get; set; }
    }
}
