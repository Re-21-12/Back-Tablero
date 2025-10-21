using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public RolController(IAdminService adminService) 
        {
            _adminService = adminService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RolDto>>> Get()
        {
            var roles = await _adminService.GetAllRolesAsync();
            var rolesDto = roles.Select(r => new RolDto(r.Id_Rol, r.Nombre));
            return Ok(rolesDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RolDto>> Get(int id)
        {
            var rol = await _adminService.GetRolByIdAsync(id);
            if (rol == null)
                return NotFound();

            var roldto = new RolDto(
                rol.Id_Rol,
                rol.Nombre
            );
            return Ok(roldto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateRolDto rolDto)
        {
            var creado = await _adminService.CreateRolAsync(rolDto);
            return CreatedAtAction(nameof(Get), new { id = creado.Id_Rol }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RolDto roldto)
        {
            var actualizado = await _adminService.UpdateRolAsync(id, roldto);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _adminService.DeleteRolAsync(id);
            if (!result)
                return NotFound("Rol no encontrado");
            
            return Ok("Rol eliminado");
        }

        [HttpPost("{parentRoleName}/permisos")]
        public async Task<IActionResult> AsignarPermisos(string parentRoleName, [FromBody] List<string> permissionNames)
        {
            var result = await _adminService.AsignarPermisosAsync(parentRoleName, permissionNames);
            if (!result)
                return BadRequest("Error al asignar permisos al rol");
            
            return Ok("Permisos asignados al rol");
        }

        [HttpPost("{parentRoleName}/permisos/{permissionName}")]
        public async Task<IActionResult> CrearPermisoEnRol(string parentRoleName, string permissionName)
        {
            var result = await _adminService.CreatePermisoEnRolAsync(parentRoleName, permissionName);
            if (!result)
                return BadRequest("Error al crear permiso en el rol");
            
            return CreatedAtAction(nameof(ObtenerPermisoDeRol), new { parentRoleName, permissionName }, "Permiso creado en el rol");
        }

        [HttpGet("{parentRoleName}/permisos/{permissionName}")]
        public async Task<ActionResult<PermisoDto>> ObtenerPermisoDeRol(string parentRoleName, string permissionName)
        {
            var permiso = await _adminService.GetPermisoDeRolAsync(parentRoleName, permissionName);
            if (permiso == null)
                return NotFound("Permiso no encontrado en el rol");
            
            return Ok(permiso);
        }

        [HttpPut("{parentRoleName}/permisos/{permissionName}")]
        public async Task<IActionResult> ActualizarPermisoDeRol(string parentRoleName, string permissionName, [FromBody] string nuevoNombre)
        {
            var result = await _adminService.ActualizarPermisoDeRolAsync(parentRoleName, permissionName, nuevoNombre);
            if (!result)
                return BadRequest("Error al actualizar el permiso del rol");
            
            return Ok("Permiso actualizado");
        }

        [HttpDelete("{parentRoleName}/permisos/{permissionName}")]
        public async Task<IActionResult> QuitarPermiso(string parentRoleName, string permissionName)
        {
            var result = await _adminService.QuitarPermisoAsync(parentRoleName, permissionName);
            if (!result)
                return NotFound("Error al quitar el permiso del rol");
            
            return Ok("Permiso removido del rol");
        }
    }
}
