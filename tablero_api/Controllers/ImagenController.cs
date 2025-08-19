using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagenController : ControllerBase
    {
        private readonly IService<Imagen> _service;

        public ImagenController(IService<Imagen> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImagenDto>>> Get()
        {
            var imagenes = await _service.GetAllAsync();
            var result = imagenes.Select(i => new ImagenDto(i.id_Imagen, i.url));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImagenDto>> Get(int id)
        {
            var imagen = await _service.GetByIdAsync(id);
            if (imagen == null)
                return NotFound();

            var dto = new ImagenDto(imagen.id_Imagen, imagen.url);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateImagenDto dto)
        {
            var imagen = new Imagen
            {
                url = dto.url
            };
            var creada = await _service.CreateAsync(imagen);
            return CreatedAtAction(nameof(Get), new { id = creada.id_Imagen }, creada);
        }
    }
}