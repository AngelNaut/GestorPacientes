using GestorPacientes.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.Interfaces.Repositories
{
    public interface IPacienteRepository : IGenericRepository<Paciente>
    {
        Task<List<Paciente>> GetAllWithDetailsAsync();
        Task<Paciente> GetByIdWithDetailsAsync(int id);
    }
}
