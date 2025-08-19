using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuartoController : ControllerBase
    {
        private readonly IService<Cuarto> _service;

        public CuartoController(IService<Cuarto> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuartoDto>>> Get()
        {
            var cuartos = await _service.GetAllAsync();
            var result = cuartos.Select(c => new CuartoDto(
                c.id_Cuarto,
                c.No_Cuarto,
                c.Total_Punteo,
                c.Total_Faltas,
                c.id_Partido,
                c.id_Equipo
            ));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CuartoDto>> Get(int id)
        {
            var cuarto = await _service.GetByIdAsync(id);
            if (cuarto == null)
                return NotFound();

            var dto = new CuartoDto(
                cuarto.id_Cuarto,
                cuarto.No_Cuarto,
                cuarto.Total_Punteo,
                cuarto.Total_Faltas,
                cuarto.id_Partido,
                cuarto.id_Equipo
            );
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCuartoDto dto)
        {
            var cuarto = new Cuarto
            {
                No_Cuarto = dto.No_Cuarto,
                Total_Punteo = dto.Total_Punteo,
                Total_Faltas = dto.Total_Faltas,
                id_Partido = dto.id_Partido,
                id_Equipo = dto.id_Equipo
            };
            var creado = await _service.CreateAsync(cuarto);
            return CreatedAtAction(nameof(Get), new { id = creado.id_Cuarto }, creado);
        }
    }
}