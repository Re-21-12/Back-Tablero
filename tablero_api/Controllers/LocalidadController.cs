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
    public class LocalidadController : ControllerBase
    {
        private readonly IService<Localidad> _service;

        public LocalidadController(IService<Localidad> service)
        {
            _service = service;
        }

        // GET: api/Localidad
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CreatedLocalidadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CreatedLocalidadDto>>> Get()
        {
            var localidades = await _service.GetAllAsync();

            var dto = localidades.Select(l => new CreatedLocalidadDto(
                l.id_Localidad,
                l.Nombre
            ));

            return Ok(dto);
        }

        // GET: api/Localidad/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CreatedLocalidadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreatedLocalidadDto>> Get(int id)
        {
            var localidad = await _service.GetByIdAsync(id);
            if (localidad == null)
                return NotFound();

            var dto = new CreatedLocalidadDto(
                localidad.id_Localidad,
                localidad.Nombre
            );

            return Ok(dto);
        }

        // POST: api/Localidad
        [HttpPost]
        [ProducesResponseType(typeof(CreatedLocalidadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<CreatedLocalidadDto>> Post([FromBody] LocalidadDto lc)
        {
            var localidad = new Localidad
            {
                Nombre = lc.Nombre
            };

            await _service.CreateAsync(localidad);

            var outDto = new CreatedLocalidadDto(localidad.id_Localidad, localidad.Nombre);

            return CreatedAtAction(nameof(Get), new { id = outDto.id }, outDto);
        }

        // PUT: api/Localidad/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CreatedLocalidadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreatedLocalidadDto>> Put(int id, [FromBody] CreatedLocalidadDto lc)
        {
            if (id != lc.id)
                return BadRequest("ID no coincide");

            var localidad = new Localidad
            {
                id_Localidad = lc.id,
                Nombre = lc.Nombre
            };

            var actualizado = await _service.UpdateAsync(localidad);

            var dto = new CreatedLocalidadDto(actualizado.id_Localidad, actualizado.Nombre);

            return Ok(dto);
        }

        // DELETE: api/Localidad/5
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var localidad = await _service.GetByIdAsync(id);
            if (localidad == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
