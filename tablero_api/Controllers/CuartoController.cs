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
        private readonly IService<Cuarto> _cuartoService;
        private readonly IService<Equipo> _equipoService;


        public CuartoController(IService<Cuarto> cuartoService, IService<Equipo> equipoService)
        {
            _cuartoService = cuartoService;
            _equipoService= equipoService;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuartoDto>>> Get()
        {
            var cuartos = await _cuartoService.GetAllAsync();
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
        public async Task<ActionResult<ResponseCuarto>> Get(int id)
        {
            var cuarto = await _cuartoService.GetByIdAsync(id);
            var equipo =  await _equipoService.GetByIdAsync(cuarto.id_Equipo) ;
            if (cuarto == null || equipo == null)
                return NotFound();

            var dto = new ResponseCuarto(
                cuarto.No_Cuarto,
                cuarto.Total_Punteo,
                cuarto.Total_Faltas,
                equipo != null ? equipo.Nombre : "Desconocido"
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
            var creado = await _cuartoService.CreateAsync(cuarto);
            return CreatedAtAction(nameof(Get), new { id = creado.id_Cuarto }, creado);
        }

    }
}