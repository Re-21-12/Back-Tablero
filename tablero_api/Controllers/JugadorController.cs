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
    public class JugadorController : ControllerBase
    {
        private readonly IService<Jugador> _service;

        public JugadorController(IService<Jugador> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreateJugadorDto>>> Get()
        {

            var jugadores = await _service.GetAllAsync();

            var dto = jugadores.Select(j => new CreateJugadorDto(
                j.Nombre,
                j.Apellido,
                j.Edad,
                j.Equipo.id_Equipo
                ));
            return Ok(dto);
        }

            [HttpGet("{id}")]
        public async Task<ActionResult<CreateJugadorDto>> Get(int id)
        {
            var jugador = await _service.GetByIdAsync(id);
            if (jugador == null)
                return NotFound();

            var dto = new CreateJugadorDto(
                jugador.Nombre,
                jugador.Apellido,
                jugador.Edad,
                jugador.Equipo.id_Equipo
                );
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateJugadorDto jugador)
        {
            var dto = new Jugador
            {
                Nombre = jugador.Nombre,
                Apellido = jugador.Apellido,
                Edad = jugador.Edad,
                id_Equipo = jugador.id_Equipo

            };
            var creado = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = creado.id_Jugador }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateJugadorDto jugadorDto)
        { 

            var jugador = await _service.GetByIdAsync(id);
            if(jugador == null) 

                return BadRequest("El ID no coincide");

            var mapJugador = new Jugador()
            {
                id_Jugador = id,
                Nombre = jugadorDto.Nombre,
                Apellido = jugadorDto.Apellido,
                Edad = jugadorDto.Edad,
                id_Equipo = jugadorDto.id_Equipo

            };

            var actualizado = await _service.UpdateAsync(mapJugador);
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
