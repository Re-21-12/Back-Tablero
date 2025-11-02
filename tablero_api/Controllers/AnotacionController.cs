using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
// a
namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnotacionController : ControllerBase
    {

        private readonly IService<Anotacion> _anotacion;
        public AnotacionController(IService<Anotacion> anotacion)
        {
            _anotacion = anotacion;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Anotacion>>> Get()
        {
            var anotaciones = await _anotacion.GetAllAsync();

            return Ok(anotaciones);

        }

        // GET api/<AnotacionController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Anotacion>> Get(int id)
        {
            Anotacion? ant = await _anotacion.GetByIdAsync(id);
            AnotacionDto att = new AnotacionDto(ant.id_jugador, ant.id_partido, ant.id_cuarto, ant.total_anotaciones);
            return Ok(att);
        }

        // POST api/<AnotacionController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AnotacionDto dto)
        {
            await _anotacion.CreateAsync(new Anotacion
            {
                id_jugador = dto.id_jugador,
                id_partido = dto.id_partido,
                id_cuarto = dto.id_cuarto,
                total_anotaciones = dto.total_anotaciones
            });
            return Ok();

        }
        [HttpGet("jugador/{id}")]
        public async Task<ActionResult<IEnumerable<AnotacionJugadorDto>>> GetByJugador(int id)
        {
            var todos = await _anotacion.GetAllAsync();
            var anotacionJugador = new List<AnotacionJugadorDto>();
            foreach (Anotacion a in todos)
            {
                if (a.id_jugador == id)
                {
                    anotacionJugador.Add(new AnotacionJugadorDto(a.id_partido, a.id_cuarto, a.total_anotaciones));
                }
            }

            return Ok(anotacionJugador);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] AnotacionDto dto)
        {
            var Antiguo = await _anotacion.GetByIdAsync(id);
            if (Antiguo == null)
            {
                return NotFound();
            }
            Antiguo.id_jugador = dto.id_jugador;
            Antiguo.total_anotaciones = dto.total_anotaciones;
            Antiguo.id_cuarto = dto.id_cuarto;
            Antiguo.id_partido = dto.id_partido;
            await _anotacion.UpdateAsync(Antiguo);
            return Ok();
        }

        // DELETE api/<AnotacionController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)

        {
            await _anotacion.DeleteAsync(id);
            return Ok();
        }
    }
}
