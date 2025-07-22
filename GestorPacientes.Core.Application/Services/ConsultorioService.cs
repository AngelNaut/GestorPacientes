using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels.Citas;
using GestorPacientes.Core.Application.ViewModels.Consultorios;
using GestorPacientes.Core.Domain.Entities;


namespace GestorPacientes.Core.Application.Services
{
    public class ConsultorioService : IConsultorioService
    {
        private readonly IConsultorioRepository _consultorioRepository;

        public ConsultorioService(IConsultorioRepository consultorioRepository)
        {
            _consultorioRepository = consultorioRepository;
        }

        public async Task<SaveConsultorioViewModel> CreateAsync(SaveConsultorioViewModel model)
        {
            var entity = new Consultorio
            {
                Nombre = model.Nombre
            };

            // Se guarda la entidad en la base de datos y se actualiza con los valores generados, como el Id.
            entity = await _consultorioRepository.AddAsync(entity);

            SaveConsultorioViewModel entityVm = new SaveConsultorioViewModel
            {
                Id = entity.Id,
                Nombre = entity.Nombre
            };

            return entityVm;
        }


        public async Task UpdateAsync(EditConsultorioViewModel model)
        {
            var consultorio = await _consultorioRepository.GetByIdAsync(model.Id);
            if (consultorio == null) return;

            consultorio.Nombre = model.Nombre;
            await _consultorioRepository.UpdateAsync(consultorio);
        }

        public async Task DeleteAsync(int id)
        {
            var consultorio = await _consultorioRepository.GetByIdAsync(id);
            if (consultorio != null)
            {
                await _consultorioRepository.DeleteAsync(consultorio);
            }
        }

        public async Task<List<ConsultorioViewModel>> GetAllAsync()
        {
            var consultorios = await _consultorioRepository.GetAllWithDetailsAsync();
            return consultorios.Select(c => new ConsultorioViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre
            }).ToList();
        }

        public async Task<ConsultorioViewModel> GetByIdAsync(int id)
        {
            var consultorio = await _consultorioRepository.GetByIdWithDetailsAsync(id);
            if (consultorio == null) return null;

            return new ConsultorioViewModel
            {
                Id = consultorio.Id,
                Nombre = consultorio.Nombre
            };
        }

        public Task UpdateAsync(EditCitaViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
