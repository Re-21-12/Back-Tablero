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
    public class EquipoController : ControllerBase
    {
        private readonly IService<Equipo> _service;

        public EquipoController(IService<Equipo> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipo>>> Get()
        {
            var equipos = await _service.GetAllAsync();
            return Ok(equipos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Equipo>> Get(int id)
        {
            var equipo = await _service.GetByIdAsync(id);
            if (equipo == null)
                return NotFound();
            return Ok(equipo);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateEquipoDto dto)
        {
            var equipo = new Equipo
            {
                Nombre = dto.Nombre,
                id_Localidad = dto.id_Localidad
            };
            await _service.CreateAsync(equipo);
            return Ok("Equipo agregado");
        }
    }
}