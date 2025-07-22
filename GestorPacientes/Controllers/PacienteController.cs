using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels.Pacientes;
using GestorPacientes.Core.Application.ViewModels.Usuarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GestorPacientes.Controllers
{
    public class PacienteController : Controller
    {
        private readonly IPacienteService _pacienteService;

        public PacienteController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        public async Task<IActionResult> Index()
        {
            var pacienteList = await _pacienteService.GetAllAsync();
            return View(pacienteList);
        }

        public IActionResult Create()
        {
            var vm = new SavePacienteViewModel();
            return View("CreatePaciente", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SavePacienteViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("CreatePaciente");

            // Llamar al servicio para crear el paciente
            SavePacienteViewModel pacienteVm = await _pacienteService.CreateAsync(vm);
            if (pacienteVm != null && pacienteVm.Id > 0)
            {
                // Subir la imagen
                pacienteVm.Foto = UploadFile(vm.File, pacienteVm.Id);

                // Mapear a EditPacienteViewModel
                EditPacienteViewModel editPacienteVm = new EditPacienteViewModel
                {
                    Id = pacienteVm.Id,
                    Nombre = pacienteVm.Nombre,
                    Apellido = pacienteVm.Apellido,
                    Telefono = pacienteVm.Telefono,
                    Direccion = pacienteVm.Direccion,
                    Cedula = pacienteVm.Cedula,
                    FechaNacimiento = pacienteVm.FechaNacimiento,
                    EsFumador = pacienteVm.EsFumador,
                    TieneAlergias = pacienteVm.TieneAlergias,
                    Foto = pacienteVm.Foto,
                    ConsultorioId = pacienteVm.ConsultorioId
                };

                await _pacienteService.UpdateAsync(editPacienteVm);
            }

            return RedirectToRoute(new { controller = "Paciente", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View("EditPaciente", await _pacienteService.GetByIdEditViewModelAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPacienteViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("EditPaciente", vm);

            EditPacienteViewModel pacienteVm = await _pacienteService.GetByIdEditViewModelAsync(vm.Id);
            vm.Foto = EditUploadFile(vm.File, vm.Id, pacienteVm.Foto);

            await _pacienteService.UpdateAsync(vm);
            return RedirectToRoute(new { controller = "Paciente", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            return View("DeletePaciente", await _pacienteService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _pacienteService.DeleteAsync(id);

            // Eliminación de archivos de imagen
            string basePath = $"/Images/Pacientes/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");
            if (Directory.Exists(path))
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

            return RedirectToRoute(new { controller = "Paciente", action = "Index" });
        }

        private string UploadFile(IFormFile file, int id)
        {
            string basePath = $"/Images/Pacientes/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            // Crear la carpeta si no existe
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

            return Path.Combine(basePath, filename);
        }

        private string EditUploadFile(IFormFile file, int id, string imageUrl = "")
        {
            if (file == null)
                return imageUrl;

            string basePath = $"/Images/Pacientes/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            // Crear la carpeta si no existe
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

            // Eliminar imagen antigua si existe
            if (!string.IsNullOrEmpty(imageUrl))
            {
                string[] oldImagePart = imageUrl.Split("/");
                string oldImageName = oldImagePart[oldImagePart.Length - 1];
                string completeImageOldPath = Path.Combine(path, oldImageName);
                if (System.IO.File.Exists(completeImageOldPath))
                {
                    System.IO.File.Delete(completeImageOldPath);
                }
            }

            return $"{basePath}/{filename}";
        }
    }
}
