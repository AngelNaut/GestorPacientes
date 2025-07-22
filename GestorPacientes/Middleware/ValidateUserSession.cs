using GestorPacientes.Core.Application.ViewModels.Usuarios;
using GestorPacientes.Core.Application.Helpers;

namespace GestorPacientes.Middleware
{
    public class ValidateUserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidateUserSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public bool HasUser(){
            UsuarioViewModel usuarioViewModel = _httpContextAccessor.HttpContext.Session.Get<UsuarioViewModel>("user");
            if (usuarioViewModel == null)
            {
                return false;
            }
            return true;
        }
    }
}
