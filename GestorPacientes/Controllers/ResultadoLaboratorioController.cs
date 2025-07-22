using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels.ResultadoLaboratorios;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorPacientes.Controllers
{
    public class ResultadoLaboratorioController : Controller
    {
        private readonly IResultadoLaboratorioService _resultadoService;

        public ResultadoLaboratorioController(IResultadoLaboratorioService resultadoService)
        {
            _resultadoService = resultadoService;
        }

        public async Task<IActionResult> Index(string cedula)
        {
            List<ResultadoLaboratorioViewModel> resultados =
                await _resultadoService.GetPendingResultsAsync(cedula);

            return View(resultados);
        }

        public async Task<IActionResult> Reportar(int id)
        {
            var resultado = await _resultadoService.GetByIdAsync(id);
            if (resultado == null)
            {
                return NotFound();
            }

            var editViewModel = new EditResultadoLaboratorioViewModel
            {
                Id = resultado.Id,
                Resultado = resultado.Resultado,
                Estado = resultado.Estado,
                CitaId = resultado.CitaId,
                PruebaLaboratorioId = resultado.PruebaLaboratorioId,
                ConsultorioId = resultado.ConsultorioId
            };

            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Reportar(EditResultadoLaboratorioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            vm.Estado = "Completada";
            await _resultadoService.UpdateAsync(vm);

            return RedirectToAction("Index");
        }
    }
}
