using GestorPacientes.Core.Application.Helpers;
using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Core.Application.ViewModels.Login;
using GestorPacientes.Core.Domain.Entities;
using GestorPacientes.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Infraestructure.Persistence.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        private readonly GestorPacienteContext _context;

        public UsuarioRepository(GestorPacienteContext context) : base(context)
        {
            _context = context;
        }
        public override async Task<Usuario> AddAsync(Usuario entity)
        {
            entity.Contrasena = PasswordEncryption.ComputeSha256Hash(entity.Contrasena);
            await _context.Set<Usuario>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<List<Usuario>> GetAllWithDetailsAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Consultorio)
                .ToListAsync();
        }

        public async Task<Usuario> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Consultorio)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> GetByUsernameAsync(string username)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == username);
        }

        public async Task<Usuario> LoginAsync(LoginViewModel loginVm)
        {
            string PasswordEncypy = PasswordEncryption.ComputeSha256Hash(loginVm.Contrasena);
            Usuario usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == loginVm.NombreUsuario && u.Contrasena == PasswordEncypy);
            return usuario;
        }
    }
}
