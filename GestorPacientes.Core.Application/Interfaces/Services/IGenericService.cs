using GestorPacientes.Core.Application.ViewModels.Citas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.Interfaces.Services
{
    public interface IGenericService<T, saveT, editT>
    {
        Task<saveT> CreateAsync(saveT model);
        Task DeleteAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task UpdateAsync(editT model);
    }
}
