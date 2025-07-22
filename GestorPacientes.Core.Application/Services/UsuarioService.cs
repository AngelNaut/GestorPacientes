using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.ViewModels;
using GestorPacientes.Core.Application.ViewModels.Login;
using GestorPacientes.Core.Application.ViewModels.Registers;
using GestorPacientes.Core.Application.ViewModels.Usuarios;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Core.Application.Helpers;
using Microsoft.AspNetCore.Http;

namespace GestorPacientes.Core.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UsuarioViewModel _usuarioViewModel;
        private readonly IConsultorioRepository _consultorioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository,
                              IConsultorioRepository consultorioRepository, IHttpContextAccessor httpContextAccessor)
        {
            _usuarioRepository = usuarioRepository;
            _consultorioRepository = consultorioRepository;
            _httpContextAccessor = httpContextAccessor;
            _usuarioViewModel = _httpContextAccessor.HttpContext.Session.Get<UsuarioViewModel>("user");
        }

        public async Task<SaveUsuarioViewModel> CreateAsync(SaveUsuarioViewModel model)
        {
            var entity = new Usuario
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Correo = model.Correo,
                NombreUsuario = model.NombreUsuario,
                Contrasena = model.Contrasena,  // ⚠️ Recomendado encriptar antes de almacenar
                TipoUsuario = model.TipoUsuario,
                FotoRuta = model.FotoRuta,
                ConsultorioId = model.ConsultorioId,
                UserId = _usuarioViewModel.ConsultorioId, // ⚠️ ¿Es correcto asignar ConsultorioId aquí?
            };

            // Guarda la entidad en la base de datos y obtiene el usuario con su ID generado
            entity = await _usuarioRepository.AddAsync(entity);

            // Mapea la entidad guardada a un ViewModel y lo retorna
            SaveUsuarioViewModel entityVm = new SaveUsuarioViewModel
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                Correo = entity.Correo,
                NombreUsuario = entity.NombreUsuario,
                TipoUsuario = entity.TipoUsuario,
                FotoRuta = entity.FotoRuta,
                ConsultorioId = entity.ConsultorioId,
                UserId = entity.UserId
            };

            return entityVm;
        }


        public async Task UpdateAsync(EditUsuarioViewModel model)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(model.Id);
            if (usuario == null) return;

            usuario.Nombre = model.Nombre;
            usuario.Apellido = model.Apellido;
            usuario.Correo = model.Correo;
            usuario.NombreUsuario = model.NombreUsuario;

            // Solo si el usuario desea cambiar la contraseña
            if (!string.IsNullOrWhiteSpace(model.Contrasena))
            {
                usuario.Contrasena = model.Contrasena; // Encriptar en un caso real
            }

            usuario.TipoUsuario = string.IsNullOrEmpty(model.TipoUsuario) ? usuario.TipoUsuario : model.TipoUsuario;
            usuario.FotoRuta = string.IsNullOrEmpty(model.FotoRuta) ? usuario.FotoRuta : model.FotoRuta;

            await _usuarioRepository.UpdateAsync(usuario);
        }

        

        public async Task<UsuarioViewModel> Login(LoginViewModel model)
        {
            Usuario usuario = await _usuarioRepository.LoginAsync(model);
            if (usuario == null) 
            {

                return null;
            }
            UsuarioViewModel usuarioVm = new();

            usuarioVm.Id = usuario.Id;
            usuarioVm.Nombre = usuario.Nombre;
            usuarioVm.Apellido = usuario.Apellido;
            usuarioVm.Correo = usuario.Correo;
            usuarioVm.NombreUsuario = usuario.NombreUsuario;
            usuarioVm.TipoUsuario = usuario.TipoUsuario;
            usuarioVm.FotoRuta = usuario.FotoRuta;
            usuarioVm.ConsultorioId = usuario.ConsultorioId;

            return usuarioVm;


        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario != null)
            {
                await _usuarioRepository.DeleteAsync(usuario);
            }
        }

        public async Task<List<UsuarioViewModel>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllWithDetailsAsync();

            // Verifica si _usuarioViewModel es null antes de usarlo
            if (_usuarioViewModel == null)
            {
                return new List<UsuarioViewModel>(); // Retorna una lista vacía en caso de error
            }

            var listViewModels = usuarios
                .Where(usuario => usuario.UserId == _usuarioViewModel.ConsultorioId)
                .Select(usuario => new UsuarioViewModel
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Correo = usuario.Correo,
                    NombreUsuario = usuario.NombreUsuario,
                    TipoUsuario = usuario.TipoUsuario,
                    FotoRuta = usuario.FotoRuta,
                    ConsultorioId = usuario.ConsultorioId
                })
                .ToList();

            return listViewModels;
        }

        public async Task<UsuarioViewModel> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdWithDetailsAsync(id);
            if (usuario == null) return null;

            return new UsuarioViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                NombreUsuario = usuario.NombreUsuario,
                TipoUsuario = usuario.TipoUsuario,
                FotoRuta = usuario.FotoRuta,
                ConsultorioId = usuario.ConsultorioId
            };
        }

     
  

        // Registro inicial de administrador y creación de consultorio
        public async Task RegisterAdminAsync(RegisterViewModel registerViewModel)
        {
            // 1. Crear el consultorio
            var consultorio = new Consultorio
            {
                Nombre = registerViewModel.NombreConsultorio
            };
            await _consultorioRepository.AddAsync(consultorio);

            // 2. Crear el usuario (admin) y asignarle el consultorio
            var usuarioAdmin = new Usuario
            {
                Nombre = registerViewModel.Nombre,
                Apellido = registerViewModel.Apellido,
                Correo = registerViewModel.Correo,
                NombreUsuario = registerViewModel.NombreUsuario,
                Contrasena = registerViewModel.Contrasena,
                TipoUsuario = "Administrador",
                ConsultorioId = consultorio.Id,
                UserId = consultorio.Id
            };
            await _usuarioRepository.AddAsync(usuarioAdmin);
        }

        public async Task<EditUsuarioViewModel> GetByIdEditViewModelAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdWithDetailsAsync(id);
            if (usuario == null) return null;

            return new EditUsuarioViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                NombreUsuario = usuario.NombreUsuario,
                TipoUsuario = usuario.TipoUsuario,
                FotoRuta = usuario.FotoRuta,
                ConsultorioId = usuario.ConsultorioId
            };
        }
    }
}
