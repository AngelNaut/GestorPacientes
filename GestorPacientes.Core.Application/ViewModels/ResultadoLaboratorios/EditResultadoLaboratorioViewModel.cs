using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.ViewModels.ResultadoLaboratorios
{
    public class EditResultadoLaboratorioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El resultado es obligatorio.")]
        [StringLength(1000, ErrorMessage = "El resultado no puede superar los 500 caracteres.")]
        public string Resultado { get; set; }

        // "Pendiente" o "Completada"
        public string Estado { get; set; }

       
        public int CitaId { get; set; }

        
        public int PruebaLaboratorioId { get; set; }

        // Asignado automáticamente
        public int ConsultorioId { get; set; }
    }
}
