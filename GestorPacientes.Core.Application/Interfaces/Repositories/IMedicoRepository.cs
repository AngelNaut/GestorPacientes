using GestorPacientes.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.Interfaces.Repositories
{
    public interface IMedicoRepository : IGenericRepository<Medico>
    {
        Task<List<Medico>> GetAllWithDetailsAsync();
        Task<Medico> GetByIdWithDetailsAsync(int id);
    }
}
