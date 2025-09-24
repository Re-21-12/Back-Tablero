using tablero_api.Models;

namespace tablero_api.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetUserWithRoleAsync(string nombre, string contrasena);
        Task<Usuario?> GetUserWithRoleAndPermissionsAsync(string nombre, string contrasena);
        Task<Usuario?> GetByUsernameAsync(string nombre);
        Task<Usuario?> GetByIdAsync(int id);
        Task AddAsync(Usuario usuario);
    }
}
