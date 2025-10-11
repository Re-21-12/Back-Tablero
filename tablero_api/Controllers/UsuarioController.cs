using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public UsuarioController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _adminService.GetAllUsuariosAsync();
            if (usuarios == null)
                return NotFound("No existen usuarios");

            return Ok(new { usuarios = usuarios });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var usuario = await _adminService.GetUsuarioByIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            return Ok(new { usuario = usuario });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioDto request)
        {
            try
            {
                var actualizado = await _adminService.UpdateUsuarioAsync(id, request);
                return Ok(actualizado);
            }
            catch (Exception)
            {
                return BadRequest("Error al actualizar el usuario");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _adminService.DeleteUsuarioAsync(id);
            if (!result)
                return NotFound("Usuario no encontrado");

            return Ok("Usuario eliminado");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioDto request)
        {
            var result = await _adminService.RegisterUsuarioAsync(request);
            if (result == null)
                return BadRequest("Error al registrar el usuario. Verifique los datos proporcionados");

            return Ok(new { message = result });
        }

        // Métodos adicionales para gestión de permisos de usuarios
        [HttpPost("{userId}/permisos")]
        public async Task<IActionResult> AsignarPermisos(string userId, [FromBody] List<string> permissionNames)
        {
            var result = await _adminService.AsignarPermisosAUsuarioAsync(userId, permissionNames);
            if (!result)
                return BadRequest("Error al asignar permisos al usuario");

            return Ok("Permisos asignados al usuario");
        }

        [HttpDelete("{userId}/permisos")]
        public async Task<IActionResult> QuitarPermisos(string userId, [FromBody] List<string> permissionNames)
        {
            var result = await _adminService.QuitarPermisosDeUsuarioAsync(userId, permissionNames);
            if (!result)
                return BadRequest("Error al quitar permisos del usuario");

            return Ok("Permisos removidos del usuario");
        }

        [HttpGet("{userId}/permisos")]
        public async Task<ActionResult<IEnumerable<PermisoDto>>> GetPermisos(string userId)
        {
            var permisos = await _adminService.GetPermisosDeUsuarioAsync(userId);
            if (permisos == null)
                return NotFound("Usuario no encontrado o sin permisos");

            return Ok(permisos);
        }

        [HttpGet("{userId}/tiene/{roleName}")]
        public async Task<ActionResult<bool>> TienePermiso(string userId, string roleName)
        {
            var tienePermiso = await _adminService.UsuarioTienePermisoAsync(userId, roleName);
            return Ok(new { userId, roleName, tienePermiso });
        }
    }
}
