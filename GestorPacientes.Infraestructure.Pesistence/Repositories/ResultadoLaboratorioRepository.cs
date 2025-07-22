using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Application.ViewModels.ResultadoLaboratorios;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Infraestructure.Persistence.Repositories
{
    public class ResultadoLaboratorioRepository : GenericRepository<ResultadoLaboratorio>, IResultadoLaboratorioRepository
    {
        private readonly GestorPacienteContext _context;

        public ResultadoLaboratorioRepository(GestorPacienteContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ResultadoLaboratorio>> GetAllWithDetailsAsync()
        {
            return await _context.ResultadosLaboratorio
                .Include(r => r.Cita)
                    .ThenInclude(c => c.Paciente) // 🔹 Incluir datos del paciente
                .Include(r => r.PruebaLaboratorio)
                .Include(r => r.Consultorio)
                .ToListAsync();
        }


        public async Task<ResultadoLaboratorio> GetByIdWithDetailsAsync(int id)
        {
            return await _context.ResultadosLaboratorio
                .Include(r => r.Cita)
                    .ThenInclude(c => c.Paciente) // 🔹 Incluir datos del paciente
                .Include(r => r.PruebaLaboratorio)
                .Include(r => r.Consultorio)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        
    }
}

