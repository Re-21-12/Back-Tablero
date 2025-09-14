using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JugadorController : ControllerBase
    {
        private readonly IService<Jugador> _service;

        public JugadorController(IService<Jugador> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jugador>>> Get()
        {
            var jugadores = await _service.GetAllAsync();
            return Ok(jugadores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Jugador>> Get(int id)
        {
            var jugador = await _service.GetByIdAsync(id);
            if (jugador == null)
                return NotFound();
            return Ok(jugador);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Jugador jugador)
        {
            var creado = await _service.CreateAsync(jugador);
            return CreatedAtAction(nameof(Get), new { id = creado.id_Jugador }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Jugador jugador)
        {
            if (id != jugador.id_Jugador)
                return BadRequest("El ID no coincide");

            var actualizado = await _service.UpdateAsync(jugador);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var jugador = await _service.GetByIdAsync(id);
            if (jugador == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return Ok("Jugador eliminado");
        }
    }
}
