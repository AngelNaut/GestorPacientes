using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels.Login;
using GestorPacientes.Core.Application.ViewModels.Registers;
using GestorPacientes.Core.Application.ViewModels.Usuarios;
using Microsoft.AspNetCore.Mvc;
using GestorPacientes.Core.Application.Helpers;

namespace GestorPacientes.Controllers
{

    public class UsuarioController : Controller
    {

        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userList = await _usuarioService.GetAllAsync();
            return View(userList);
        }

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();


        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UsuarioViewModel userVm= await _usuarioService.Login(model);

            if (userVm != null)
            {
                HttpContext.Session.Set<UsuarioViewModel>("user", userVm);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("userValidation", "Datos de acceso incorrecto");
            }

            return View(model);


        }

        
        public IActionResult Register()
        {
            // Muestra la vista con el formulario
            return View();
        }
     

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Aquí llamas a tu servicio para crear el consultorio y el usuario
            await _usuarioService.RegisterAdminAsync(model);

            // Redirigir o mostrar la vista que desees
            return RedirectToAction("login", "Usuario");
        }

        public IActionResult Create()
        {
            var vm = new SaveUsuarioViewModel();
            return View(vm); // busca Views/Usuario/Create.cshtml
        }

       


        [HttpPost]
        public async Task<IActionResult> Create(SaveUsuarioViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Obtener el usuario administrador actual desde la sesión (asegúrate de tenerlo almacenado previamente)
            var currentAdmin = HttpContext.Session.Get<UsuarioViewModel>("user");

            if (currentAdmin == null)
            {
                // Maneja el caso en que no se encuentre el usuario actual, por ejemplo, redirige a login o muestra un error
                return RedirectToAction("login");
            }

            // Si el usuario que se está creando es de tipo "Administrador", asignar el mismo ConsultorioId
            if (vm.TipoUsuario == "Administrador" || vm.TipoUsuario == "Asistente")
            {
                vm.ConsultorioId = currentAdmin.ConsultorioId;
            }
            

            // Llamar al servicio para crear el usuario
            await _usuarioService.CreateAsync(vm);

           
            return RedirectToRoute(new { controller = "Usuario", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {

            return View("EditUsuario", await _usuarioService.GetByIdEditViewModelAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(EditUsuarioViewModel vm)
        {
            

            if (!ModelState.IsValid)
            {

                
                return View("EditUsuario", vm);
            }

            await _usuarioService.UpdateAsync(vm);
            return RedirectToRoute(new { controller = "Usuario", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {

            return View("DeleteUsuario",await _usuarioService.GetByIdAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {


            await _usuarioService.DeleteAsync(id);
            return RedirectToRoute(new { controller = "Usuario", action = "Index" });
        }

        public IActionResult LogOut()
        {

            HttpContext.Session.Remove("user");
            return RedirectToAction("Login", "Usuario");
        }

    }
}

