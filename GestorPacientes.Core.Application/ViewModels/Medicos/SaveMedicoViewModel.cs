using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.ViewModels.Medicos
{
    public class SaveMedicoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        public string Cedula { get; set; }

       
        
        public string? Foto { get; set; }

        // Para relacionar con el consultorio del usuario actual (Administrador)
        public int ConsultorioId { get; set; }
        
        public IFormFile? File { get; set; }
    }
}
