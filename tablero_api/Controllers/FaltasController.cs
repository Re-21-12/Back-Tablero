using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaltasController : ControllerBase
    {
        private readonly IService<Falta> _faltas;
        public FaltasController(IService<Falta> faltas)
        {
            _faltas = faltas;
        }

        // GET: api/<FaltasController>
        [HttpGet]
        public async Task<ActionResult< IEnumerable<FaltaDto>>> Get()
        {
            var todos = await _faltas.GetAllAsync();
            List<FaltaDto> faltas = new List<FaltaDto>();
            foreach(Falta f in todos){
                faltas.Add(new FaltaDto(f.id_jugador, f.id_partido, f.id_cuarto, f.total_falta));
            }
            return Ok(faltas);
        }

        // GET api/<FaltasController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FaltaDto>> Get(int id)
        {
            var entero = await _faltas.GetByIdAsync(id);
            FaltaDto falta = new FaltaDto(entero.id_jugador, entero.id_partido, entero.id_cuarto, entero.total_falta);
            return Ok(falta);
        }

        // POST api/<FaltasController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] FaltaDto dto)
        {
            Falta falta = new Falta();
            falta.total_falta = dto.total_faltas;
            falta.id_jugador = dto.id_Jugador;
            falta.id_partido = dto.Id_Partido;
            falta.id_cuarto = dto.Id_Cuarto;
            await _faltas.CreateAsync(falta);
            return Ok();
        }

        // PUT api/<FaltasController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] FaltaDto dto)
        {
            var Antiguo = await _faltas.GetByIdAsync(id);
            if (Antiguo == null)
            {
                return NotFound();
            }
            Antiguo.id_jugador = dto.id_Jugador;
            Antiguo.total_falta = dto.total_faltas;
            Antiguo.id_cuarto = dto.Id_Cuarto;
            Antiguo.id_partido = dto.Id_Partido;
            await _faltas.UpdateAsync(Antiguo);
            return Ok();
        }

        // DELETE api/<FaltasController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _faltas.DeleteAsync(id);
            return Ok();
        }
    }
}
