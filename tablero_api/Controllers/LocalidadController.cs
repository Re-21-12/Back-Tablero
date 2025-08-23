using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Services.Interfaces;
using System.Threading.Tasks;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalidadController : ControllerBase
    {
        private readonly IService<Localidad> _service;

        public LocalidadController(IService<Localidad> service)
        {
            _service = service;
        }

        // GET: api/<LocalidadController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Localidad>>> Get()
        {
            var localidades = await _service.GetAllAsync();
            return Ok(localidades);
        }

        // GET api/<LocalidadController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Localidad>> Get(int id)
        {
            var localidad = await _service.GetByIdAsync(id);
            if (localidad == null)
                return NotFound();
            return Ok(localidad);
        }

        // POST api/<LocalidadController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Localidad lc)
        {
            await _service.CreateAsync(lc);
            return Ok("Localidad agregada");
        }

        // PUT api/<LocalidadController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Localidad lc)
        {
            if (id != lc.id_Localidad)
                return BadRequest("ID no coincide");

            await _service.UpdateAsync(lc);
            return Ok("Localidad actualizada");
        }

        // DELETE api/<LocalidadController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var localidad = await _service.GetByIdAsync(id);
            if (localidad == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return Ok("Localidad eliminada");
        }
    }
}