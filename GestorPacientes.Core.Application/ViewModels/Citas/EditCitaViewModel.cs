using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.ViewModels.Citas
{
    public class EditCitaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaCita { get; set; }

        [Required(ErrorMessage = "La hora de la cita es obligatoria.")]
        [DataType(DataType.Time)]
        public TimeSpan HoraCita { get; set; }

        [Required(ErrorMessage = "La causa es obligatoria.")]
        [StringLength(500, ErrorMessage = "La causa no puede superar los 500 caracteres.")]
        public string Causa { get; set; }

        // Estado: "PendienteConsulta", "PendienteResultados", "Completada"
        // Normalmente se asigna por la lógica de negocio, pero se puede exponer si deseas
        public string Estado { get; set; }

        // Seleccionar en la vista
        [Required(ErrorMessage = "Debe seleccionar un paciente.")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un médico.")]
        public int MedicoId { get; set; }

        // Se asigna automáticamente el consultorio del asistente logueado
        public int ConsultorioId { get; set; }
    }
}
