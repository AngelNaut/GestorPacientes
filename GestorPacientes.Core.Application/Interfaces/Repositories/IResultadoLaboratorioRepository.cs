using GestorPacientes.Core.Application.ViewModels.ResultadoLaboratorios;
using GestorPacientes.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.Interfaces.Repositories
{
    public interface IResultadoLaboratorioRepository : IGenericRepository<ResultadoLaboratorio>
    {
        Task<List<ResultadoLaboratorio>> GetAllWithDetailsAsync();
        Task<ResultadoLaboratorio> GetByIdWithDetailsAsync(int id);
        
        
    }
}
