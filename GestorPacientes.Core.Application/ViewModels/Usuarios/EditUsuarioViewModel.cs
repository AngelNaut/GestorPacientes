using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.ViewModels.Usuarios
{
    public class EditUsuarioViewModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreUsuario { get; set; }


        public string? Contrasena { get; set; }


        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmarContrasena { get; set; }

        public string? TipoUsuario { get; set; } // "Administrador" o "Asistente"
        public string? FotoRuta { get; set; }
        public int ConsultorioId { get; set; }


    }
}
