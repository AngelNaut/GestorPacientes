using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GestorPacientes.Infraestructure.Persistence.Repositories
{
    internal class MedicoRepository : GenericRepository<Medico>, IMedicoRepository
    {
        private readonly GestorPacienteContext _context;

        public MedicoRepository(GestorPacienteContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Medico>> GetAllWithDetailsAsync()
        {
            return await _context.Medicos
                .Include(m => m.Consultorio)
                .Include(m => m.Citas)
                .ToListAsync();
        }

        public async Task<Medico> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Medicos
                .Include(m => m.Consultorio)
                .Include(m => m.Citas)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
