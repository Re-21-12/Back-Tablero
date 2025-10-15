using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EquipoController : ControllerBase
    {
        private readonly IService<Equipo> _service;
        private readonly IService<Localidad> _localidadService;
        private readonly HttpClient _httpClient;



        public EquipoController(IService<Equipo> service, IService<Localidad> localidadService, HttpClient httpClient)
        {
            _service = service;
            _localidadService = localidadService;
            _httpClient = httpClient;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipoDto>>> Get()
        {
            var equipos = await _service.GetAllAsync();
            var equipoDtos = new List<EquipoDto>();

            foreach (var equipo in equipos)
            {
                var localidad = await _localidadService.GetByIdAsync(equipo.id_Localidad);
                equipoDtos.Add(new EquipoDto(
                    equipo.id_Equipo,
                    equipo.Nombre,
                    localidad?.Nombre ?? string.Empty,
                    localidad.id_Localidad,
                    equipo.url_imagen
                ));
            }

            return Ok(equipoDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipoDto>> Get(int id)
        {
            var equipo = await _service.GetByIdAsync(id);
            if (equipo == null)
                return NotFound();

            var localidad = await _localidadService.GetByIdAsync(equipo.id_Localidad);

            var dto = new EquipoDto
            (
                equipo.id_Equipo,
                equipo.Nombre,
                localidad?.Nombre ?? string.Empty,
                localidad.id_Localidad,
                equipo.url_imagen

            );
            return Ok(dto);
        }
        [HttpGet("reporte")]
        public async Task<IActionResult> GetReporte()
        {
            string python_string = "http://127.0.0.1:5001/Reporte/Equipos";
            var todos = await _service.GetAllAsync();
            var equipoDtos = new List<EquipoDto>();



            JsonElement listaEquipos;
            foreach (var equipo in todos)
            {
                var localidad = await _localidadService.GetByIdAsync(equipo.id_Localidad);
                equipoDtos.Add(new EquipoDto(
                    equipo.id_Equipo,
                    equipo.Nombre,
                    localidad?.Nombre ?? string.Empty,
                    localidad.id_Localidad,
                    equipo.url_imagen
                ));
            }
            var json = JsonSerializer.Serialize(equipoDtos);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(python_string, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                return BadRequest($"Error del servicio Python: {errorMsg}");
            }
            var pdfBytes = await response.Content.ReadAsByteArrayAsync();


            return File(pdfBytes, "application/pdf", "reporte_equipos.pdf");
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

        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] EquipoImageDto dto)
        {
            var equipo = await _service.GetByIdAsync(dto.id_Equipo);
            {
                equipo.url_imagen = dto.url;
            }
            await _service.UpdateAsync(equipo);
            {
                return Ok("Imagen agregada");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateEquipoDto? equipoDto)
        {
            var equipo = await _service.GetByIdAsync(id);
            if (equipo == null)
                return BadRequest("ID no encontrado");

            var mapEquipo = new Equipo()
            {
                Nombre = equipoDto.Nombre,
                id_Localidad = equipoDto.id_Localidad
            };

            var actualizado = await _service.UpdateAsync(mapEquipo);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var equipo = await _service.GetByIdAsync(id);
            if (equipo == null)
                return NotFound();
            await _service.DeleteAsync(id);
            return Ok("Jugador eliminado");
        }


        [HttpGet("Paginado")]
        public async Task<Pagina<EquipoDto>> GetEquiposAsync([FromQuery] int pagina = 1, [FromQuery] int tamanio = 10)
        {
            var todos = await _service.GetAllAsync();
            var equipos = await _service.GetValuePerPage(pagina, tamanio);
            List<EquipoDto> eq = new List<EquipoDto>();

            foreach (Equipo i in equipos)
            {
                var localidad = await _localidadService.GetByIdAsync(i.id_Localidad);
                eq.Add(new EquipoDto(i.id_Equipo, i.Nombre, localidad?.Nombre ?? string.Empty, localidad.id_Localidad, i.url_imagen));
            }

            return new Pagina<EquipoDto>
            {
                Items = eq,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(todos.Count() / (double)tamanio),
                TotalRegistros = todos.Count()
            };
        }

    }
}