using GestorPacientes.Core.Application.ViewModels.Login;
using GestorPacientes.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<List<Usuario>> GetAllWithDetailsAsync();
        Task<Usuario> GetByIdWithDetailsAsync(int id);
        Task<Usuario> GetByUsernameAsync(string username);
        Task<Usuario> LoginAsync(LoginViewModel loginVm);
    }
}
