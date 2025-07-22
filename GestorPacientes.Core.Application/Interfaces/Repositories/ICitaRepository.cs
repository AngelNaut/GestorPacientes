using GestorPacientes.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.Interfaces.Repositories
{
    public interface ICitaRepository : IGenericRepository<Cita>
    {
        Task<List<Cita>> GetAllWithDetailsAsync();
        Task<Cita> GetByIdWithDetailsAsync(int id);
    }
}
