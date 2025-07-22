using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.ViewModels.Usuarios
{
    public class SaveUsuarioViewModel
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

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }


        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; }

        [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
        public string TipoUsuario { get; set; } // "Administrador" o "Asistente"

        // Si deseas manejar la foto en el usuario
        public string? FotoRuta { get; set; }

        // Para asignar o mostrar a qué consultorio pertenece (opcional en la vista)
        public int ConsultorioId { get; set; }
    }
}
