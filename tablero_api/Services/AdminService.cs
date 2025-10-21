using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Services
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AdminService> _logger;

        public AdminService(HttpClient httpClient, ILogger<AdminService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Métodos del UsuarioController - Ajustados para Keycloak
        public async Task<IEnumerable<Usuario>?> GetAllUsuariosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/keycloak/users");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<Usuario>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios del microservicio");
                return null;
            }
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/keycloak/users/{id}");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Usuario>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {UserId} del microservicio", id);
                return null;
            }
        }

        public async Task<Usuario> UpdateUsuarioAsync(int id, UsuarioDto request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/keycloak/users/{id}", content);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Usuario>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {UserId} en el microservicio", id);
                throw;
            }
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/keycloak/users/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UserId} del microservicio", id);
                return false;
            }
        }

        public async Task<string?> RegisterUsuarioAsync(UsuarioDto request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/keycloak/users", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario en el microservicio");
                return null;
            }
        }

        // Métodos del RolController - Ajustados para Keycloak
        public async Task<IEnumerable<Rol>> GetAllRolesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/keycloak/roles");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<Rol>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? Enumerable.Empty<Rol>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles del microservicio");
                throw;
            }
        }

        public async Task<Rol?> GetRolByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/keycloak/roles/{id}");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Rol>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol {RolId} del microservicio", id);
                return null;
            }
        }

        public async Task<Rol> CreateRolAsync(CreateRolDto rolDto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(rolDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/keycloak/roles", content);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Rol>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear rol en el microservicio");
                throw;
            }
        }

        public async Task<Rol> UpdateRolAsync(int id, RolDto rolDto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(rolDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/keycloak/roles/{id}", content);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Rol>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol {RolId} en el microservicio", id);
                throw;
            }
        }

        public async Task<bool> DeleteRolAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/keycloak/roles/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar rol {RolId} del microservicio", id);
                return false;
            }
        }

        // Métodos de Permisos para Roles - Corregidos según OpenAPI de Keycloak
        public async Task<bool> CreatePermisoEnRolAsync(string parentRoleName, string permissionName)
        {
            try
            {
                var createPermissionRequest = new { name = permissionName };
                var jsonContent = JsonSerializer.Serialize(createPermissionRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Endpoint: POST /keycloak/permissions/roles/{parentRoleName}
                var response = await _httpClient.PostAsync($"/keycloak/permissions/roles/{parentRoleName}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear permiso {PermissionName} en el rol {RolName}", permissionName, parentRoleName);
                return false;
            }
        }

        public async Task<bool> AsignarPermisosAsync(string parentRoleName, List<string> permissionNames)
        {
            try
            {
                // Construir la URL con los nombres de permisos como parámetro de path
                var permissionNamesParam = string.Join(",", permissionNames);

                // Endpoint: POST /keycloak/permissions/roles/{parentRoleName}/{permissionNames}
                var response = await _httpClient.PostAsync($"/keycloak/permissions/roles/{parentRoleName}/{permissionNamesParam}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar permisos al rol {RolName} en el microservicio", parentRoleName);
                return false;
            }
        }

        public async Task<bool> QuitarPermisoAsync(string parentRoleName, string permissionName)
        {
            try
            {
                // Endpoint: DELETE /keycloak/permissions/roles/{parentRoleName}/{permissionName}
                var response = await _httpClient.DeleteAsync($"/keycloak/permissions/roles/{parentRoleName}/{permissionName}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al quitar permiso {PermissionName} del rol {RolName} en el microservicio", permissionName, parentRoleName);
                return false;
            }
        }

        public async Task<PermisoDto?> GetPermisoDeRolAsync(string parentRoleName, string permissionName)
        {
            try
            {
                // Endpoint: GET /keycloak/permissions/roles/{parentRoleName}/{permissionName}
                var response = await _httpClient.GetAsync($"/keycloak/permissions/roles/{parentRoleName}/{permissionName}");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PermisoDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permiso {PermissionName} del rol {RolName}", permissionName, parentRoleName);
                return null;
            }
        }

        public async Task<bool> ActualizarPermisoDeRolAsync(string parentRoleName, string permissionName, string nuevoNombre)
        {
            try
            {
                var updateRequest = new { name = nuevoNombre };
                var jsonContent = JsonSerializer.Serialize(updateRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Endpoint: PUT /keycloak/permissions/roles/{parentRoleName}/{permissionName}
                var response = await _httpClient.PutAsync($"/keycloak/permissions/roles/{parentRoleName}/{permissionName}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar permiso {PermissionName} del rol {RolName}", permissionName, parentRoleName);
                return false;
            }
        }

        // Métodos del PermisoController - Ajustados para Keycloak
        public async Task<IEnumerable<PermisoDto>> GetAllPermisosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/keycloak/permissions/roles/all");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<PermisoDto>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? Enumerable.Empty<PermisoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos del microservicio");
                throw;
            }
        }

        public async Task<PermisoDto?> GetPermisoByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/keycloak/permissions/roles/{id}");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PermisoDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permiso {PermisoId} del microservicio", id);
                return null;
            }
        }

        public async Task<Permiso> CreatePermisoAsync(PermisoDto permisoDto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(permisoDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/keycloak/permissions/roles", content);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Permiso>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear permiso en el microservicio");
                throw;
            }
        }

        public async Task<Permiso> UpdatePermisoAsync(int id, PermisoDto permisoDto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(permisoDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/keycloak/permissions/roles/{id}", content);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Permiso>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar permiso {PermisoId} en el microservicio", id);
                throw;
            }
        }

        public async Task<bool> DeletePermisoAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/keycloak/permissions/roles/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permiso {PermisoId} del microservicio", id);
                return false;
            }
        }

        // Métodos para permisos de usuarios según OpenAPI
        public async Task<bool> AsignarPermisosAUsuarioAsync(string userId, List<string> permissionNames)
        {
            try
            {
                var assignRequest = new { roles = permissionNames };
                var jsonContent = JsonSerializer.Serialize(assignRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/keycloak/permissions/users/{userId}/roles", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar permisos al usuario {UserId} en el microservicio", userId);
                return false;
            }
        }

        public async Task<bool> QuitarPermisosDeUsuarioAsync(string userId, List<string> permissionNames)
        {
            try
            {
                var removeRequest = new { roles = permissionNames };
                var jsonContent = JsonSerializer.Serialize(removeRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.DeleteAsync($"/keycloak/permissions/users/{userId}/roles");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al quitar permisos del usuario {UserId} en el microservicio", userId);
                return false;
            }
        }

        public async Task<IEnumerable<PermisoDto>?> GetPermisosDeUsuarioAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/keycloak/permissions/users/{userId}");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<PermisoDto>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permisos del usuario {UserId} del microservicio", userId);
                return null;
            }
        }

        public async Task<bool> UsuarioTienePermisoAsync(string userId, string roleName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/keycloak/permissions/users/{userId}/has/{roleName}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si usuario {UserId} tiene permiso {RoleName}", userId, roleName);
                return false;
            }
        }
    }
}