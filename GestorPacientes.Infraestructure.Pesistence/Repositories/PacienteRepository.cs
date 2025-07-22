using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GestorPacientes.Infraestructure.Persistence.Repositories
{
    public class PacienteRepository : GenericRepository<Paciente>, IPacienteRepository
    {
        private readonly GestorPacienteContext _context;

        public PacienteRepository(GestorPacienteContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Paciente>> GetAllWithDetailsAsync()
        {
            return await _context.Pacientes
                .Include(p => p.Consultorio)
                .Include(p => p.Citas)
                .ToListAsync();
        }

        public async Task<Paciente> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Pacientes
                .Include(p => p.Consultorio)
                .Include(p => p.Citas)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}

