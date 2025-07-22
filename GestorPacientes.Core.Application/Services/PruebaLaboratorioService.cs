using GestorPacientes.Core.Application.Helpers;
using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels.PruebaLaboratorios;
using GestorPacientes.Core.Application.ViewModels.Usuarios;
using GestorPacientes.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.Services
{
    public class PruebaLaboratorioService : IPruebaLaboratorioService
    {
        private readonly IPruebaLaboratorioRepository _pruebaRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UsuarioViewModel _usuarioViewModel;

        public PruebaLaboratorioService(IPruebaLaboratorioRepository pruebaRepository, IHttpContextAccessor httpContextAccessor)
        {
            _pruebaRepository = pruebaRepository;
            _httpContextAccessor = httpContextAccessor;
            _usuarioViewModel = _httpContextAccessor.HttpContext.Session.Get<UsuarioViewModel>("user");
        }

        public async Task<SavePruebaLaboratorioViewModel> CreateAsync(SavePruebaLaboratorioViewModel model)
        {
            var entity = new PruebaLaboratorio
            {
                Nombre = model.Nombre,
                ConsultorioId = _usuarioViewModel.ConsultorioId
            };

            // Guarda la entidad en la base de datos
            entity = await _pruebaRepository.AddAsync(entity);

            // Mapea la entidad guardada a un ViewModel y lo retorna
            SavePruebaLaboratorioViewModel entityVm = new SavePruebaLaboratorioViewModel
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                ConsultorioId = entity.ConsultorioId
            };

            return entityVm;
        }


        public async Task UpdateAsync(EditPruebaLaboratorioViewModel model)
        {
            var prueba = await _pruebaRepository.GetByIdAsync(model.Id);
            if (prueba == null) return;

            prueba.Nombre = model.Nombre;
            await _pruebaRepository.UpdateAsync(prueba);
        }

        public async Task DeleteAsync(int id)
        {
            var prueba = await _pruebaRepository.GetByIdAsync(id);
            if (prueba != null)
            {
                await _pruebaRepository.DeleteAsync(prueba);
            }
        }

        public async Task<List<PruebaLaboratorioViewModel>> GetAllAsync()
        {
            var pruebas = await _pruebaRepository.GetAllWithDetailsAsync();
             if (_usuarioViewModel == null)
            {
                return new List<PruebaLaboratorioViewModel>(); // Retorna una lista vacía en caso de error
            }
            var listViewModels = pruebas
               .Where(pruebas => pruebas.ConsultorioId == _usuarioViewModel.ConsultorioId)
               .Select(p => new PruebaLaboratorioViewModel
               {
                Id = p.Id,
                Nombre = p.Nombre,
                ConsultorioId = p.ConsultorioId
            }).ToList();
            return listViewModels;
        }

        public async Task<PruebaLaboratorioViewModel> GetByIdAsync(int id)
        {
            var prueba = await _pruebaRepository.GetByIdWithDetailsAsync(id);
            if (prueba == null) return null;

            return new PruebaLaboratorioViewModel
            {
                Id = prueba.Id,
                Nombre = prueba.Nombre,
                ConsultorioId = prueba.ConsultorioId
            };
        }
        public async Task<EditPruebaLaboratorioViewModel> GetByIdEditViewModelAsync(int id)
        {
            var usuario = await _pruebaRepository.GetByIdWithDetailsAsync(id);
            if (usuario == null) return null;

            return new EditPruebaLaboratorioViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                ConsultorioId = usuario.ConsultorioId
            };
        }
    }
}
