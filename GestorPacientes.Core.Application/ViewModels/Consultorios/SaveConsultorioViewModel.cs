using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.ViewModels.Consultorios
{
    public class SaveConsultorioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del consultorio es obligatorio.")]
        public string Nombre { get; set; }
    }
}
