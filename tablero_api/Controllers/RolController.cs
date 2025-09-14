using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
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
        public async Task<ActionResult<IEnumerable<Rol>>> Get()
        {
            var roles = await _service.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> Get(int id)
        {
            var rol = await _service.GetByIdAsync(id);
            if (rol == null)
                return NotFound();
            return Ok(rol);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Rol rol)
        {
            var creado = await _service.CreateAsync(rol);
            return CreatedAtAction(nameof(Get), new { id = creado.Id_Rol }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Rol rol)
        {
            if (id != rol.Id_Rol)
                return BadRequest("El ID no coincide");

            var actualizado = await _service.UpdateAsync(rol);
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
