using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;

namespace tablero_api.Services.Interfaces
{
    public interface IAdminService
    {
        // Métodos del UsuarioController
        Task<IEnumerable<Usuario>?> GetAllUsuariosAsync();
        Task<Usuario?> GetUsuarioByIdAsync(int id);
        Task<Usuario> UpdateUsuarioAsync(int id, UsuarioDto request);
        Task<bool> DeleteUsuarioAsync(int id);
        Task<string?> RegisterUsuarioAsync(UsuarioDto request);

        // Métodos del RolController
        Task<IEnumerable<Rol>> GetAllRolesAsync();
        Task<Rol?> GetRolByIdAsync(int id);
        Task<Rol> CreateRolAsync(CreateRolDto rolDto);
        Task<Rol> UpdateRolAsync(int id, RolDto rolDto);
        Task<bool> DeleteRolAsync(int id);

        // Métodos de Permisos para Roles - Corregidos según OpenAPI de Keycloak
        Task<bool> CreatePermisoEnRolAsync(string parentRoleName, string permissionName);
        Task<bool> AsignarPermisosAsync(string parentRoleName, List<string> permissionNames);
        Task<bool> QuitarPermisoAsync(string parentRoleName, string permissionName);
        Task<PermisoDto?> GetPermisoDeRolAsync(string parentRoleName, string permissionName);
        Task<bool> ActualizarPermisoDeRolAsync(string parentRoleName, string permissionName, string nuevoNombre);

        // Métodos del PermisoController
        Task<IEnumerable<PermisoDto>> GetAllPermisosAsync();
        Task<PermisoDto?> GetPermisoByIdAsync(int id);
        Task<Permiso> CreatePermisoAsync(PermisoDto permisoDto);
        Task<Permiso> UpdatePermisoAsync(int id, PermisoDto permisoDto);
        Task<bool> DeletePermisoAsync(int id);

        // Métodos para permisos de usuarios según OpenAPI
        Task<bool> AsignarPermisosAUsuarioAsync(string userId, List<string> permissionNames);
        Task<bool> QuitarPermisosDeUsuarioAsync(string userId, List<string> permissionNames);
        Task<IEnumerable<PermisoDto>?> GetPermisosDeUsuarioAsync(string userId);
        Task<bool> UsuarioTienePermisoAsync(string userId, string roleName);
    }
}