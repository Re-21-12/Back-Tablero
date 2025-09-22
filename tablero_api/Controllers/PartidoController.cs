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
    public class PartidoController : ControllerBase
    {
        private readonly IService<Partido> _partidoService;
        private readonly IService<Equipo> _equipoService;
        private readonly IService<Localidad> _localidadService;


        public PartidoController(IService<Partido> partidoService, IService<Equipo> equipoService, IService<Localidad> localidadSerice)
        {
            _partidoService = partidoService;
            _equipoService = equipoService;
            _localidadService = localidadSerice;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartidoDto>>> Get()
        {
            var partidos = await _partidoService.GetAllAsync();

            var result = partidos.Select(p => new PartidoDto(
                p.FechaHora,
                p.id_Localidad,
                p.id_Local,
                p.id_Visitante
            ));
            return Ok(result);
        }
        [HttpGet("/Paginado")]
        public async Task<ActionResult<IEnumerable<PartidoDto>>> GetPartidosPerPage([FromQuery] int pagina, [FromQuery] int tamanio)
        {
            var partidos = await _partidoService.GetAllAsync();


            var result = await _partidoService.GetValuePerPage(pagina, tamanio);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PartidoDto>> Get(int id)
        {
            var partido = await _partidoService.GetByIdAsync(id);
            if (partido == null)
                return NotFound();


            var dto = new PartidoDto(
                partido.FechaHora,
                partido.id_Localidad,
                partido.id_Local,
                partido.id_Visitante
            );
            var equipoLocalNombre = await _equipoService.GetByIdAsync(partido.id_Local);
            var equipoVisitanteNombre = await _equipoService.GetByIdAsync(partido.id_Visitante);
            var localidad = await _localidadService.GetByIdAsync(partido.id_Localidad);
            var dtoResponse = new ResponsePartidoDto(
                dto.FechaHora,
                localidad != null ? localidad.Nombre : "Desconocida",
                equipoLocalNombre != null ? equipoLocalNombre.Nombre : "Desconocido",
                equipoVisitanteNombre != null ? equipoVisitanteNombre.Nombre : "Desconocido"
            );
            return Ok(dtoResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePartidoDto dto)
        {
            var equipoLocal = await _equipoService.GetByIdAsync(dto.id_Local);
            var localidad = await _localidadService.GetByIdAsync(equipoLocal.id_Localidad);

            var partido = new Partido
            {
                FechaHora = dto.FechaHora,
                id_Localidad = localidad.id_Localidad,
                id_Local = dto.id_Local,
                id_Visitante = dto.id_Visitante
            };
            var creado = await _partidoService.CreateAsync(partido);
            return CreatedAtAction(nameof(Get), new { id = creado.id_Partido }, new { id = creado.id_Partido });
        }

        [HttpPut ("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]  UpdatePartidoDto partidoDto)
        {
            var partido = await _partidoService.GetByIdAsync(id);
            if (partido == null)
                return BadRequest("ID no encontrado");

            var mapPartido = new Partido
            {
             
                FechaHora = partidoDto.FechaHora,
                id_Local = partidoDto.id_Local,
                id_Visitante = partidoDto.id_visitante
            };

            var actualizado = await _partidoService.UpdateAsync(mapPartido);
            return Ok("Partido Actualizado");

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var partido = await _partidoService.GetByIdAsync(id);
                if (partido == null)
                return NotFound();

            await _partidoService.DeleteAsync(id);
            return Ok("Partido eliminado");
        }
    }
}