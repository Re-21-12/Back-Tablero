using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
<<<<<<< HEAD
=======
using tablero_api.Services;
>>>>>>> origin/stable
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolController : ControllerBase
    {
        private readonly IService<Rol> _rolService;
        private readonly IService<Permiso> _permisoService;

        public RolController(IService<Rol> rolService,IService<Permiso> permisoService)
        {
            _rolService = rolService;
            _permisoService = permisoService;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDto>>> Get()
        {
<<<<<<< HEAD
            var roles = await _service.GetAllAsync();
            var dto = roles.Select(r => new RolDto(
                r.Nombre
                ));
            return Ok(dto);
=======
            var roles = await _rolService.GetAllAsync();
            return Ok(roles);
>>>>>>> origin/stable
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RolDto>> Get(int id)
        {
            var rol = await _rolService.GetByIdAsync(id);
            if (rol == null)
                return NotFound();

            var roldto = new RolDto(
                rol.Nombre
                );
            return Ok(roldto);
        }

        [HttpPost]
<<<<<<< HEAD
        public async Task<IActionResult> Post([FromBody] RolDto rol)
        {
            
                var roldto = new Rol
                {
                    Nombre = rol.Nombre

                };

                var creado = await _service.CreateAsync(roldto);
=======
        public async Task<IActionResult> Post([FromBody] RolDto rolDto)
        {
            var rol = new Rol()
            {
                Nombre = rolDto.Nombre
            };
            var creado = await _rolService.CreateAsync(rol);
>>>>>>> origin/stable
            return CreatedAtAction(nameof(Get), new { id = creado.Id_Rol }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RolDto roldto)
        {
            var rol = _rolService.GetByIdAsync(id);
            if (rol == null)
                return BadRequest("El ID no coincide");

            var Maprol = new Rol()
            {
                Id_Rol = id,
                Nombre = roldto.Nombre
            };

            var actualizado = await _rolService.UpdateAsync(Maprol);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rol = await _rolService.GetByIdAsync(id);
            if (rol == null)
                return NotFound();

            await _rolService.DeleteAsync(id);
            return Ok("Rol eliminado");
        }
        [HttpPost("{rolId}/permisos")]
        public async Task<IActionResult> AsignarPermisos(int rolId, [FromBody] List<int> permisosIds)
        {
            var rol = await _rolService.GetByIdAsync(rolId);
            if (rol == null)
                return NotFound("Rol no encontrado");

            rol.Permisos.Clear(); 

            foreach (var permisoId in permisosIds)
            {
                var permiso = await _permisoService.GetByIdAsync(permisoId);
                if (permiso != null && !rol.Permisos.Any(p => p.Id_Permiso == permisoId))
                    rol.Permisos.Add(permiso);
            }

            await _rolService.UpdateAsync(rol);
            return Ok("Permisos asignados al rol");
        }
        [HttpDelete("{rolId}/permisos/{permisoId}")]
        public async Task<IActionResult> QuitarPermiso(int rolId, int permisoId)
        {
            var rol = await _rolService.GetByIdAsync(rolId);
            if (rol == null)
                return NotFound("Rol no encontrado");

            var permiso = rol.Permisos.FirstOrDefault(p => p.Id_Permiso == permisoId);
            if (permiso == null)
                return NotFound("Permiso no asignado a este rol");

            rol.Permisos.Remove(permiso);
            await _rolService.UpdateAsync(rol);
            return Ok("Permiso removido del rol");
        }
    }
}
