using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels.ResultadoLaboratorios;
using GestorPacientes.Core.Application.ViewModels.Usuarios;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Core.Application.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Core.Application.Services
{
    public class ResultadoLaboratorioService : IResultadoLaboratorioService
    {
        private readonly IResultadoLaboratorioRepository _resultadoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UsuarioViewModel _usuarioViewModel;

        public ResultadoLaboratorioService(IResultadoLaboratorioRepository resultadoRepository, IHttpContextAccessor httpContextAccessor)
        {
            _resultadoRepository = resultadoRepository;
            _httpContextAccessor = httpContextAccessor;
            _usuarioViewModel = _httpContextAccessor.HttpContext.Session.Get<UsuarioViewModel>("user");
        }

        public async Task<SaveResultadoLaboratorioViewModel> CreateAsync(SaveResultadoLaboratorioViewModel model)
        {
            var entity = new ResultadoLaboratorio
            {
                Resultado = model.Resultado,
                Estado = model.Estado, // "Pendiente" o "Completada"
                CitaId = model.CitaId,
                PruebaLaboratorioId = model.PruebaLaboratorioId,
                ConsultorioId = model.ConsultorioId
            };

            // Guarda la entidad en la base de datos
            entity = await _resultadoRepository.AddAsync(entity);

            // Mapea la entidad guardada a un ViewModel y lo retorna
            SaveResultadoLaboratorioViewModel entityVm = new SaveResultadoLaboratorioViewModel
            {
                Id = entity.Id,
                Resultado = entity.Resultado,
                Estado = entity.Estado,
                CitaId = entity.CitaId,
                PruebaLaboratorioId = entity.PruebaLaboratorioId,
                ConsultorioId = entity.ConsultorioId
            };

            return entityVm;
        }


        public async Task UpdateAsync(EditResultadoLaboratorioViewModel model)
        {
            var resultado = await _resultadoRepository.GetByIdAsync(model.Id);
            if (resultado == null) return;

            resultado.Resultado = model.Resultado;
            resultado.Estado = model.Estado;
            await _resultadoRepository.UpdateAsync(resultado);
        }

        public async Task DeleteAsync(int id)
        {
            var resultado = await _resultadoRepository.GetByIdAsync(id);
            if (resultado != null)
            {
                await _resultadoRepository.DeleteAsync(resultado);
            }
        }

        public async Task<List<ResultadoLaboratorioViewModel>> GetAllAsync()
        {
            var resultados = await _resultadoRepository
                .GetAllWithDetailsAsync(); // Asegúrate de que incluye Cita y PruebaLaboratorio

            return resultados.Select(resultado => new ResultadoLaboratorioViewModel
            {
                Id = resultado.Id,
                Resultado = resultado.Resultado,
                Estado = resultado.Estado,
                CitaId = resultado.CitaId,
                PruebaLaboratorioId = resultado.PruebaLaboratorioId,
                ConsultorioId = resultado.ConsultorioId,

                // Datos del paciente obtenidos a través de la Cita
                PacienteNombre = resultado.Cita?.Paciente?.Nombre ?? "Desconocido",
                PacienteCedula = resultado.Cita?.Paciente?.Cedula ?? "Desconocido",

                // Nombre de la prueba
                PruebaNombre = resultado.PruebaLaboratorio?.Nombre ?? "Desconocido"
            }).ToList();
        }

        public async Task<ResultadoLaboratorioViewModel> GetByIdAsync(int id)
        {
            var resultado = await _resultadoRepository.GetByIdWithDetailsAsync(id);
            if (resultado == null) return null;

            return new ResultadoLaboratorioViewModel
            {
                Id = resultado.Id,
                Resultado = resultado.Resultado,
                Estado = resultado.Estado,
                CitaId = resultado.CitaId,
                PruebaLaboratorioId = resultado.PruebaLaboratorioId,
                ConsultorioId = resultado.ConsultorioId
            };
        }

        public async Task<List<ResultadoLaboratorioViewModel>> GetPendingResultsAsync(string cedula)
        {
            // Lógica para filtrar resultados pendientes:
            var resultados = await _resultadoRepository.GetAllWithDetailsAsync();

            // Si tienes un usuario logueado, obtén el ConsultorioId
            if (_usuarioViewModel == null)
                return new List<ResultadoLaboratorioViewModel>();

            var query = resultados.Where(r => r.ConsultorioId == _usuarioViewModel.ConsultorioId
                                              && r.Estado == "Pendiente");

            if (!string.IsNullOrWhiteSpace(cedula))
            {
                // Recuerda: r.Cita.Paciente.Cedula, si la entidad está incluida con .ThenInclude()
                query = query.Where(r => r.Cita.Paciente.Cedula == cedula);
            }

            // Mapea al ViewModel
            return query.Select(r => new ResultadoLaboratorioViewModel
            {
                Id = r.Id,
                Resultado = r.Resultado,
                Estado = r.Estado,
                CitaId = r.CitaId,
                PruebaLaboratorioId = r.PruebaLaboratorioId,
                ConsultorioId = r.ConsultorioId,

                // Asumiendo que incluyes Cita -> Paciente y PruebaLaboratorio
                PacienteNombre = r.Cita.Paciente.Nombre,
                PacienteCedula = r.Cita.Paciente.Cedula,
                PruebaNombre = r.PruebaLaboratorio.Nombre
            }).ToList();
        }
    }
}
