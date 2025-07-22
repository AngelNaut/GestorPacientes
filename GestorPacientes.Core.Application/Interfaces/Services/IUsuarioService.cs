using GestorPacientes.Core.Application.ViewModels.Login;
using GestorPacientes.Core.Application.ViewModels.Registers;
using GestorPacientes.Core.Application.ViewModels.Usuarios;


namespace GestorPacientes.Core.Application.Interfaces.Services
{
    public interface IUsuarioService : IGenericService<UsuarioViewModel, SaveUsuarioViewModel, EditUsuarioViewModel>
    {
      
       

        Task RegisterAdminAsync(RegisterViewModel registerViewModel);
        Task<UsuarioViewModel> Login(LoginViewModel model);
        Task<EditUsuarioViewModel> GetByIdEditViewModelAsync(int id);
    }
}