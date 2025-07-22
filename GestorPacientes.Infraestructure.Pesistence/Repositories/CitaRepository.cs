using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GestorPacientes.Infraestructure.Persistence.Repositories
{
    public class CitaRepository : GenericRepository<Cita>, ICitaRepository
    {
        private readonly GestorPacienteContext _context;

        public CitaRepository(GestorPacienteContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Cita>> GetAllWithDetailsAsync()
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Include(c => c.Consultorio)
                .ToListAsync();
        }

        public async Task<Cita> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Include(c => c.Consultorio)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }

  
}

