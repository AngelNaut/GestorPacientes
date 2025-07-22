using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels.Citas;
using GestorPacientes.Core.Domain.Entities;


namespace GestorPacientes.Core.Application.Services
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepository;

        public CitaService(ICitaRepository citaRepository)
        {
            _citaRepository = citaRepository;
        }

        public async Task<SaveCitaViewModel> CreateAsync(SaveCitaViewModel model)
        {
            var entity = new Cita
            {
                FechaCita = model.FechaCita,
                HoraCita = model.HoraCita,
                Causa = model.Causa,
                Estado = string.IsNullOrEmpty(model.Estado) ? "PendienteConsulta" : model.Estado,
                PacienteId = model.PacienteId,
                MedicoId = model.MedicoId,
                ConsultorioId = model.ConsultorioId
            };

            entity = await _citaRepository.AddAsync(entity);
            SaveCitaViewModel entityVm = new SaveCitaViewModel
            {
                Id = entity.Id,
                FechaCita = entity.FechaCita,
                HoraCita = entity.HoraCita,
                Causa = entity.Causa,
                Estado = entity.Estado,
                PacienteId = entity.PacienteId,
                MedicoId = entity.MedicoId,
                ConsultorioId = entity.ConsultorioId
            };

            return entityVm;


        }

        public async Task UpdateAsync(EditCitaViewModel model)
        {
            var cita = await _citaRepository.GetByIdAsync(model.Id);
            if (cita == null) return;

            cita.FechaCita = model.FechaCita;
            cita.HoraCita = model.HoraCita;
            cita.Causa = model.Causa;
            cita.Estado = model.Estado;
            cita.PacienteId = model.PacienteId;
            cita.MedicoId = model.MedicoId;
            // Normalmente no se cambia el ConsultorioId

            await _citaRepository.UpdateAsync(cita);
        }

        public async Task DeleteAsync(int id)
        {
            var cita = await _citaRepository.GetByIdAsync(id);
            if (cita != null)
            {
                await _citaRepository.DeleteAsync(cita);
            }
        }

        public async Task<List<CitaViewModel>> GetAllAsync()
        {
            var citas = await _citaRepository.GetAllWithDetailsAsync();
            return citas.Select(c => new CitaViewModel
            {
                Id = c.Id,
                FechaCita = c.FechaCita,
                HoraCita = c.HoraCita,
                Causa = c.Causa,
                Estado = c.Estado,
                PacienteId = c.PacienteId,
                MedicoId = c.MedicoId,
                ConsultorioId = c.ConsultorioId
            }).ToList();
        }

        public async Task<CitaViewModel> GetByIdAsync(int id)
        {
            var cita = await _citaRepository.GetByIdWithDetailsAsync(id);
            if (cita == null) return null;

            return new CitaViewModel
            {
                Id = cita.Id,
                FechaCita = cita.FechaCita,
                HoraCita = cita.HoraCita,
                Causa = cita.Causa,
                Estado = cita.Estado,
                PacienteId = cita.PacienteId,
                MedicoId = cita.MedicoId,
                ConsultorioId = cita.ConsultorioId
            };
        }

       
    }
}
