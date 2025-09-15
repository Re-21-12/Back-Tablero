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
        private readonly IService<Rol> _service;

        public RolController(IService<Rol> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDto>>> Get()
        {
            var roles = await _service.GetAllAsync();
            var dto = roles.Select(r => new RolDto(
                r.Nombre
                ));
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RolDto>> Get(int id)
        {
            var rol = await _service.GetByIdAsync(id);
            if (rol == null)
                return NotFound();

            var roldto = new RolDto(
                rol.Nombre
                );
            return Ok(roldto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RolDto rol)
        {
            
                var roldto = new Rol
                {
                    Nombre = rol.Nombre

                };

                var creado = await _service.CreateAsync(roldto);
            return CreatedAtAction(nameof(Get), new { id = creado.Id_Rol }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RolDto roldto)
        {
            var rol = _service.GetByIdAsync(id);
            if (rol == null)
                return BadRequest("El ID no coincide");

            var Maprol = new Rol()
            {
                Id_Rol = id,
                Nombre = roldto.Nombre
            };

            var actualizado = await _service.UpdateAsync(Maprol);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rol = await _service.GetByIdAsync(id);
            if (rol == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return Ok("Rol eliminado");
        }
    }
}
