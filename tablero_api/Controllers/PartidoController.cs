using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidoController : ControllerBase
    {
        private readonly IService<Partido> _service;

        public PartidoController(IService<Partido> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartidoDto>>> Get()
        {
            var partidos = await _service.GetAllAsync();
            var result = partidos.Select(p => new PartidoDto(
                p.id_Partido,
                p.FechaHora,
                p.id_Localidad,
                p.id_Local,
                p.id_Visitante
            ));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PartidoDto>> Get(int id)
        {
            var partido = await _service.GetByIdAsync(id);
            if (partido == null)
                return NotFound();

            var dto = new PartidoDto(
                partido.id_Partido,
                partido.FechaHora,
                partido.id_Localidad,
                partido.id_Local,
                partido.id_Visitante
            );
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePartidoDto dto)
        {
            var partido = new Partido
            {
                FechaHora = dto.FechaHora,
                id_Localidad = dto.id_Localidad,
                id_Local = dto.id_Local,
                id_Visitante = dto.id_Visitante
            };
            var creado = await _service.CreateAsync(partido);
            return CreatedAtAction(nameof(Get), new { id = creado.id_Partido }, creado);
        }
    }
}