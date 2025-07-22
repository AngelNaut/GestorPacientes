using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestorPacientes.Core.Application.Helpers;
using GestorPacientes.Core.Application.ViewModels.PruebaLaboratorios;
using GestorPacientes.Core.Application.ViewModels.Usuarios;

namespace GestorPacientes.Controllers
{
    public class PruebaLaboratorioController : Controller
    {
        private readonly IPruebaLaboratorioService _pruebaLaboratorioService;

        // Constructor que recibe el servicio por inyección de dependencias
        public PruebaLaboratorioController(IPruebaLaboratorioService pruebaLaboratorioService)
        {
            _pruebaLaboratorioService = pruebaLaboratorioService;
        }

        public async Task<IActionResult> Index()
        {
            var pruebaLabortorioList = await _pruebaLaboratorioService.GetAllAsync();
            return View(pruebaLabortorioList);
        }

        public IActionResult Create()
        {
            var vm = new SavePruebaLaboratorioViewModel();

            return View("CreatePruebaLaboratorio", vm);
        }




        [HttpPost]
        public async Task<IActionResult> Create(SavePruebaLaboratorioViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Obtener el usuario administrador actual desde la sesión (asegúrate de tenerlo almacenado previamente)
            var currentAdmin = HttpContext.Session.Get<PruebaLaboratorioViewModel>("user");

            if (currentAdmin == null)
            {
                // Maneja el caso en que no se encuentre el usuario actual, por ejemplo, redirige a login o muestra un error
                return RedirectToAction("login");
            }

            


            // Llamar al servicio para crear el usuario
            await _pruebaLaboratorioService.CreateAsync(vm);


            return RedirectToRoute(new { controller = "PruebaLaboratorio", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {

            return View("EditPruebaLaboratorio", await _pruebaLaboratorioService.GetByIdEditViewModelAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(EditPruebaLaboratorioViewModel vm)
        {


            if (!ModelState.IsValid)
            {


                return View("EditUsuario", vm);
            }

            await _pruebaLaboratorioService.UpdateAsync(vm);
            return RedirectToRoute(new { controller = "PruebaLaboratorio", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {

            return View("DeletePruebaLaboratorio", await _pruebaLaboratorioService.GetByIdAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {


            await _pruebaLaboratorioService.DeleteAsync(id);
            return RedirectToRoute(new { controller = "PruebaLaboratorio", action = "Index" });
        }

    }

}
