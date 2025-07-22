using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels.Medicos;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Core.Application.Helpers;
using Microsoft.AspNetCore.Http;
using GestorPacientes.Core.Application.ViewModels.Usuarios;


namespace GestorPacientes.Core.Application.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IMedicoRepository _medicoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UsuarioViewModel _usuarioViewModel;
       

        public MedicoService(IMedicoRepository medicoRepository, IHttpContextAccessor httpContextAccessor)
        {
            _medicoRepository = medicoRepository;
            _httpContextAccessor = httpContextAccessor;
            _usuarioViewModel = _httpContextAccessor.HttpContext.Session.Get<UsuarioViewModel>("user");
          
        }

        public async Task<SaveMedicoViewModel> CreateAsync(SaveMedicoViewModel model)
        {
            var entity = new Medico
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Correo = model.Correo,
                Telefono = model.Telefono,
                Cedula = model.Cedula,
                Foto = model.Foto,
                ConsultorioId = _usuarioViewModel.ConsultorioId
            };

            // Guarda la entidad y actualiza el objeto con los datos persistidos (como el ID)
            entity = await _medicoRepository.AddAsync(entity);

            // Mapea la entidad guardada a un ViewModel para retornarlo
            SaveMedicoViewModel entityVm = new SaveMedicoViewModel
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                Correo = entity.Correo,
                Telefono = entity.Telefono,
                Cedula = entity.Cedula,
                Foto = entity.Foto,
                ConsultorioId = entity.ConsultorioId
            };

            return entityVm;
        }


        public async Task UpdateAsync(EditMedicoViewModel model)
        {
            var medico = await _medicoRepository.GetByIdAsync(model.Id);
            if (medico == null) return;

            medico.Nombre = model.Nombre;
            medico.Apellido = model.Apellido;
            medico.Correo = model.Correo;
            medico.Telefono = model.Telefono;
            medico.Cedula = model.Cedula;

            if (!string.IsNullOrEmpty(model.Foto))
            {
                medico.Foto = model.Foto;
            }

            await _medicoRepository.UpdateAsync(medico);
        }

        public async Task DeleteAsync(int id)
        {
            var medico = await _medicoRepository.GetByIdAsync(id);
            if (medico != null)
            {
                await _medicoRepository.DeleteAsync(medico);
            }
        }

        public async Task<List<MedicoViewModel>> GetAllAsync()
        {
            var medicos = await _medicoRepository.GetAllWithDetailsAsync();


            var listViewModels = medicos
                .Where(medicos => medicos.ConsultorioId == _usuarioViewModel.ConsultorioId)
                .Select(m => new MedicoViewModel
                {
                Id = m.Id,
                Nombre = m.Nombre,
                Apellido = m.Apellido,
                Correo = m.Correo,
                Telefono = m.Telefono,
                Cedula = m.Cedula,
                Foto = m.Foto,
                ConsultorioId = m.ConsultorioId
            }).ToList();

            return listViewModels;
        }

        public async Task<MedicoViewModel> GetByIdAsync(int id)
        {
            var medico = await _medicoRepository.GetByIdWithDetailsAsync(id);
            if (medico == null) return null;

            return new MedicoViewModel
            {
                Id = medico.Id,
                Nombre = medico.Nombre,
                Apellido = medico.Apellido,
                Correo = medico.Correo,
                Telefono = medico.Telefono,
                Cedula = medico.Cedula,
                Foto = medico.Foto,
                ConsultorioId = medico.ConsultorioId
            };
        }

        public async Task<EditMedicoViewModel> GetByIdEditViewModelAsync(int id)
        {
            var medico = await _medicoRepository.GetByIdWithDetailsAsync(id);
            if (medico == null) return null;

            return new EditMedicoViewModel
            {
                Id = medico.Id,
                Nombre = medico.Nombre,
                Apellido = medico.Apellido,
                Correo = medico.Correo,
                Telefono = medico.Telefono,
                Cedula = medico.Cedula,
                Foto = medico.Foto,
                ConsultorioId = medico.ConsultorioId
            };
        }
    }
}
