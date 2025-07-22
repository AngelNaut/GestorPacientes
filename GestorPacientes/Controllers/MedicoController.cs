using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.Services;
using GestorPacientes.Core.Application.ViewModels.Usuarios;
using Microsoft.AspNetCore.Http;
using GestorPacientes.Core.Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using GestorPacientes.Core.Application.ViewModels.Medicos;

namespace GestorPacientes.Controllers
{
    public class MedicoController : Controller
    {
        IMedicoService _medicoService;
        public MedicoController(IMedicoService medicoService)
        {
            _medicoService = medicoService;
        }
       
           public async Task<IActionResult> Index()
        {
            var medicoList = await _medicoService.GetAllAsync();
            return View(medicoList);
        }

        public IActionResult Create()
        {
            var vm = new SaveMedicoViewModel();

            return View("CreateMedico", vm);
        }




        [HttpPost]
        public async Task<IActionResult> Create(SaveMedicoViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("CreateMedico");

            // Obtener el usuario administrador actual desde la sesión (asegúrate de tenerlo almacenado previamente)
            var currentAdmin = HttpContext.Session.Get<UsuarioViewModel>("user");

            if (currentAdmin == null)
            {
                // Maneja el caso en que no se encuentre el usuario actual, por ejemplo, redirige a login o muestra un error
                return RedirectToAction("login");
            }




            // Llamar al servicio para crear el usuario
            SaveMedicoViewModel medicoVm = await _medicoService.CreateAsync(vm);
            if (medicoVm != null && medicoVm.Id > 0)
            {
                // Subir la imagen
                medicoVm.Foto = UploadFile(vm.File, medicoVm.Id);

                // Mapear a EditMedicoViewModel
                EditMedicoViewModel editMedicoVm = new EditMedicoViewModel
                {
                    Id = medicoVm.Id,
                    Nombre = medicoVm.Nombre,
                    Apellido = medicoVm.Apellido,
                    Correo = medicoVm.Correo,
                    Telefono = medicoVm.Telefono,
                    Cedula = medicoVm.Cedula,
                    Foto = medicoVm.Foto,
                    ConsultorioId = medicoVm.ConsultorioId
                    // Asigna otras propiedades según corresponda
                };

                await _medicoService.UpdateAsync(editMedicoVm);
            }

            return RedirectToRoute(new { controller = "Medico", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {

            return View("EditMedico", await _medicoService.GetByIdEditViewModelAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(EditMedicoViewModel vm)
        {


            if (!ModelState.IsValid)
            {


                return View("EditMedico", vm);
            }
            EditMedicoViewModel medicoVm = await _medicoService.GetByIdEditViewModelAsync(vm.Id);
            vm.Foto = EditUploadFile(vm.File, vm.Id, medicoVm.Foto);

            await _medicoService.UpdateAsync(vm);
            return RedirectToRoute(new { controller = "Medico", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {

            return View("DeleteMedico", await _medicoService.GetByIdAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {


            await _medicoService.DeleteAsync(id);
            string basePath = $"/Images/Medicos/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");
            if(Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo folder in directoryInfo.GetDirectories())
                {
                    folder.Delete(true);
                }
                Directory.Delete(path);
            }
            return RedirectToRoute(new { controller = "Medico", action = "Index" });
        }

        private string UploadFile(IFormFile file, int id ) 
        {
            string basePath = $"/Images/Medicos/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            //create folder if not exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Guid guid = Guid.NewGuid();
            FileInfo fileInfo = new(file.FileName);
            string filename = guid + fileInfo.Extension;
            string fullnameWithPath = Path.Combine(path, filename);

            using(var stream = new FileStream(fullnameWithPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Path.Combine(basePath, filename);
        }

        private string EditUploadFile(IFormFile file, int id, string imageUrl = "")
        {
            if(file == null)
            {
                return imageUrl;
            }
            string basePath = $"/Images/Medicos/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            //create folder if not exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Guid guid = Guid.NewGuid();
            FileInfo fileInfo = new(file.FileName);
            string filename = guid + fileInfo.Extension;
            string fullnameWithPath = Path.Combine(path, filename);

            using (var stream = new FileStream(fullnameWithPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            string[] oldImagePart = imageUrl.Split("/");
            string oldImageName = oldImagePart[oldImagePart.Length - 1];
            string completeImageOldPath = Path.Combine(path, oldImageName);

            if(System.IO.File.Exists(completeImageOldPath))
            {
                System.IO.File.Delete(completeImageOldPath);
            }
            return $"{basePath}/{filename}";
        }

    }
}
