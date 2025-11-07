using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermisoController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public PermisoController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PermisoDto>>> Get()
        {
            var permisos = await _adminService.GetAllPermisosAsync();
            return Ok(permisos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermisoDto>> Get(int id)
        {
            var permiso = await _adminService.GetPermisoByIdAsync(id);
            if (permiso == null)
                return NotFound();

            return Ok(permiso);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PermisoDto permisoDto)
        {
            var creado = await _adminService.CreatePermisoAsync(permisoDto);
            return CreatedAtAction(nameof(Get), new { id = creado.Id_Permiso }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PermisoDto permisoDto)
        {
            var actualizado = await _adminService.UpdatePermisoAsync(id, permisoDto);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _adminService.DeletePermisoAsync(id);
            if (!result)
                return NotFound("Permiso no encontrado");

            return Ok("Permiso eliminado");
        }

        // Métodos adicionales para gestión de permisos de usuarios
        [HttpPost("usuarios/{userId}")]
        public async Task<IActionResult> AsignarPermisosAUsuario(string userId, [FromBody] List<string> permissionNames)
        {
            var result = await _adminService.AsignarPermisosAUsuarioAsync(userId, permissionNames);
            if (!result)
                return BadRequest("Error al asignar permisos al usuario");

            return Ok("Permisos asignados al usuario");
        }

        [HttpDelete("usuarios/{userId}")]
        public async Task<IActionResult> QuitarPermisosDeUsuario(string userId, [FromBody] List<string> permissionNames)
        {
            var result = await _adminService.QuitarPermisosDeUsuarioAsync(userId, permissionNames);
            if (!result)
                return BadRequest("Error al quitar permisos del usuario");

            return Ok("Permisos removidos del usuario");
        }

        [HttpGet("usuarios/{userId}")]
        public async Task<ActionResult<IEnumerable<PermisoDto>>> GetPermisosDeUsuario(string userId)
        {
            var permisos = await _adminService.GetPermisosDeUsuarioAsync(userId);
            if (permisos == null)
                return NotFound("Usuario no encontrado o sin permisos");

            return Ok(permisos);
        }

        [HttpGet("usuarios/{userId}/tiene/{roleName}")]
        public async Task<ActionResult<bool>> UsuarioTienePermiso(string userId, string roleName)
        {
            var tienePermiso = await _adminService.UsuarioTienePermisoAsync(userId, roleName);
            return Ok(new { userId, roleName, tienePermiso });
        }
    }
}
