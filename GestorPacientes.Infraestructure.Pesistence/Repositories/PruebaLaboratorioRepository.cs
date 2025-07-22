using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GestorPacientes.Infraestructure.Persistence.Repositories
{
    public class PruebaLaboratorioRepository : GenericRepository<PruebaLaboratorio>, IPruebaLaboratorioRepository
    {
        private readonly GestorPacienteContext _context;

        public PruebaLaboratorioRepository(GestorPacienteContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<PruebaLaboratorio>> GetAllWithDetailsAsync()
        {
            return await _context.PruebasLaboratorio
                .Include(p => p.Consultorio)
                .Include(p => p.ResultadosLaboratorio)
                .ToListAsync();
        }

        public async Task<PruebaLaboratorio> GetByIdWithDetailsAsync(int id)
        {
            return await _context.PruebasLaboratorio
                .Include(p => p.Consultorio)
                .Include(p => p.ResultadosLaboratorio)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}

