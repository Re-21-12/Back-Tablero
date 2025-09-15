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
        private readonly IService<Permiso> _service;

        public PermisoController(IService<Permiso> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermisoDto>>> Get()
        {
            var permisos = await _service.GetAllAsync();
            var dto = permisos.Select(p => new PermisoDto(
                p.Nombre
                ));
           
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermisoDto>> Get(int id)
        {
            var permiso = await _service.GetByIdAsync(id);
            if (permiso == null)
                return NotFound();
            var dto = new PermisoDto
            (
               permiso.Nombre
            );
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PermisoDto permiso)
        {
            {
                var dto = new Permiso
                {
                    Nombre = permiso.Nombre

                };
                var creado = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = creado.Id_Permiso }, creado);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PermisoDto permisodto)
        {
            var permiso = await _service.GetByIdAsync(id);
            if (permiso == null) 
                return BadRequest("El ID no coincide");

            var mapPermiso = new Permiso()
            {
                Id_Permiso = id,
                Nombre = permisodto.Nombre
            };

            var actualizado = await _service.UpdateAsync(mapPermiso);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var permiso = await _service.GetByIdAsync(id);
            if (permiso == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return Ok("Permiso eliminado");
        }
    }
}
