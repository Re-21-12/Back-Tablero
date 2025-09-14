using Microsoft.EntityFrameworkCore;
using tablero_api.Data;
using tablero_api.Models;
using tablero_api.Repositories.Interfaces;

namespace tablero_api.Repositories
{
    public class UsuarioRepository(AppDbContext context) 
        : IUsuarioRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<Usuario?> GetUserWithRoleAsync(string nombre, string contrasena)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Nombre == nombre);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena))
                return null;
            return usuario;
        }
        public async Task<Usuario?> GetByUsernameAsync(string nombre)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre== nombre);
        }

        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
