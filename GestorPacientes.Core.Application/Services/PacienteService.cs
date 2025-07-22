using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels.Pacientes;
using GestorPacientes.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using GestorPacientes.Core.Application.ViewModels.Usuarios;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorPacientes.Core.Application.Helpers;

namespace GestorPacientes.Core.Application.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UsuarioViewModel _usuarioViewModel;

        public PacienteService(IPacienteRepository pacienteRepository, IHttpContextAccessor httpContextAccessor)
        {
            _pacienteRepository = pacienteRepository;
            _httpContextAccessor = httpContextAccessor;
            _usuarioViewModel = _httpContextAccessor.HttpContext.Session.Get<UsuarioViewModel>("user");
        }

        public async Task<SavePacienteViewModel> CreateAsync(SavePacienteViewModel model)
        {
            var entity = new Paciente
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Telefono = model.Telefono,
                Direccion = model.Direccion,
                Cedula = model.Cedula,
                FechaNacimiento = model.FechaNacimiento,
                EsFumador = model.EsFumador,
                TieneAlergias = model.TieneAlergias,
                Foto = model.Foto,
                // Se asigna el ConsultorioId del usuario logueado
                ConsultorioId = _usuarioViewModel.ConsultorioId
            };

            // Guarda la entidad y la actualiza con el ID generado
            entity = await _pacienteRepository.AddAsync(entity);

            // Mapea la entidad guardada a un ViewModel para retornarlo
            SavePacienteViewModel entityVm = new SavePacienteViewModel
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                Telefono = entity.Telefono,
                Direccion = entity.Direccion,
                Cedula = entity.Cedula,
                FechaNacimiento = entity.FechaNacimiento,
                EsFumador = entity.EsFumador,
                TieneAlergias = entity.TieneAlergias,
                Foto = entity.Foto,
                ConsultorioId = entity.ConsultorioId
            };

            return entityVm;
        }

        public async Task UpdateAsync(EditPacienteViewModel model)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(model.Id);
            if (paciente == null) return;

            paciente.Nombre = model.Nombre;
            paciente.Apellido = model.Apellido;
            paciente.Telefono = model.Telefono;
            paciente.Direccion = model.Direccion;
            paciente.Cedula = model.Cedula;
            paciente.FechaNacimiento = model.FechaNacimiento;
            paciente.EsFumador = model.EsFumador;
            paciente.TieneAlergias = model.TieneAlergias;

            if (!string.IsNullOrEmpty(model.Foto))
            {
                paciente.Foto = model.Foto;
            }

            await _pacienteRepository.UpdateAsync(paciente);
        }

        public async Task DeleteAsync(int id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);
            if (paciente != null)
            {
                await _pacienteRepository.DeleteAsync(paciente);
            }
        }

        public async Task<List<PacienteViewModel>> GetAllAsync()
        {
            var pacientes = await _pacienteRepository.GetAllWithDetailsAsync();

            var listViewModels = pacientes
                .Where(p => p.ConsultorioId == _usuarioViewModel.ConsultorioId)
                .Select(p => new PacienteViewModel
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Telefono = p.Telefono,
                    Direccion = p.Direccion,
                    Cedula = p.Cedula,
                    FechaNacimiento = p.FechaNacimiento,
                    EsFumador = p.EsFumador,
                    TieneAlergias = p.TieneAlergias,
                    Foto = p.Foto,
                    ConsultorioId = p.ConsultorioId
                }).ToList();

            return listViewModels;
        }

        public async Task<PacienteViewModel> GetByIdAsync(int id)
        {
            var paciente = await _pacienteRepository.GetByIdWithDetailsAsync(id);
            if (paciente == null) return null;

            return new PacienteViewModel
            {
                Id = paciente.Id,
                Nombre = paciente.Nombre,
                Apellido = paciente.Apellido,
                Telefono = paciente.Telefono,
                Direccion = paciente.Direccion,
                Cedula = paciente.Cedula,
                FechaNacimiento = paciente.FechaNacimiento,
                EsFumador = paciente.EsFumador,
                TieneAlergias = paciente.TieneAlergias,
                Foto = paciente.Foto,
                ConsultorioId = paciente.ConsultorioId
            };
        }

        // Método adicional para obtener el EditPacienteViewModel, similar a GetByIdEditViewModelAsync en MedicoService
        public async Task<EditPacienteViewModel> GetByIdEditViewModelAsync(int id)
        {
            var paciente = await _pacienteRepository.GetByIdWithDetailsAsync(id);
            if (paciente == null) return null;

            return new EditPacienteViewModel
            {
                Id = paciente.Id,
                Nombre = paciente.Nombre,
                Apellido = paciente.Apellido,
                Telefono = paciente.Telefono,
                Direccion = paciente.Direccion,
                Cedula = paciente.Cedula,
                FechaNacimiento = paciente.FechaNacimiento,
                EsFumador = paciente.EsFumador,
                TieneAlergias = paciente.TieneAlergias,
                Foto = paciente.Foto,
                ConsultorioId = paciente.ConsultorioId
            };
        }
    }
}

