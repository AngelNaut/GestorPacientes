using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GestorPacientes.Infraestructure.Persistence.Repositories
{
    public class ConsultorioRepository : GenericRepository<Consultorio>, IConsultorioRepository
    {
        private readonly GestorPacienteContext _context;

        public ConsultorioRepository(GestorPacienteContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Consultorio>> GetAllWithDetailsAsync()
        {
            return await _context.Consultorios
                .Include(c => c.Usuarios)
                .Include(c => c.Medicos)
                .Include(c => c.Pacientes)
                .Include(c => c.PruebasLaboratorio)
                .Include(c => c.Citas)
                .Include(c => c.ResultadosLaboratorio)
                .ToListAsync();
        }

        public async Task<Consultorio> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Consultorios
                .Include(c => c.Usuarios)
                .Include(c => c.Medicos)
                .Include(c => c.Pacientes)
                .Include(c => c.PruebasLaboratorio)
                .Include(c => c.Citas)
                .Include(c => c.ResultadosLaboratorio)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
