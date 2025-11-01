using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using tablero_api.Migrations;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;
using Microsoft.Extensions.Configuration; // <-- agregado

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JugadorController : ControllerBase
    {
        private readonly IService<Jugador> _service;
        private readonly IService<Equipo> _EquipoService;
        private readonly IService<Falta> _faltas;
        private readonly IService<Anotacion> _anotaciones;
        private readonly HttpClient _httpClient;
        private readonly string _reportServiceBaseUrl; // <-- agregado

        public JugadorController(IService<Jugador> service, IService<Equipo> equipoService, HttpClient httpClient, IService<Falta> faltas, IService<Anotacion> anotaciones, IConfiguration configuration)
        {
            _service = service;
            _EquipoService = equipoService;
            _httpClient = httpClient;
            _faltas = faltas;
            _anotaciones = anotaciones;
            _reportServiceBaseUrl = configuration.GetValue<string>("MicroServices:ReportService") ?? "http://127.0.0.1:5001";
        }
        [HttpGet("byTeam/{id_equipo}")]
        public async Task<ActionResult<IEnumerable<JugadorDto>>> GetByTeam(int id_equipo)
        {
            var equipo = await _EquipoService.GetByIdAsync(id_equipo);
            var jugadores = await _service.GetAllAsync();
            List<JugadorDto> ret = new List<JugadorDto>();
            foreach (Jugador j in jugadores)
            {
                if (j.id_Equipo == id_equipo)
                {
                    ret.Add(new JugadorDto(j.Nombre, j.Apellido, j.Estatura, j.Posicion, j.Nacionalidad, j.Edad, j.id_Equipo));
                }
            }


            if (equipo == null)
                return NotFound("No existe el equipo");
            return Ok(ret);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreateJugadorDto>>> Get()
        {

            var jugadores = await _service.GetAllAsync();

            var dto = jugadores.Select(j => new CreateJugadorDto(
                j.Nombre,
                j.Apellido,
                j.Estatura,
                j.Posicion,
                j.Nacionalidad,
                j.Edad,
                j.id_Equipo
            ));
            return Ok(dto);
        }
        [HttpGet("Reporte/Equipo")]
        public async Task<IActionResult> GetReporte([FromQuery] int id_equipo)
        {
            var baseUrl = _reportServiceBaseUrl.TrimEnd('/');
            var todos = await _service.GetAllAsync();
            var equipo = await _EquipoService.GetByIdAsync(id_equipo);
            if (equipo == null)
                return NotFound("Equipo no encontrado");

            var jugadores = new List<JugadorDto>();
            var python_string = $"{baseUrl}/Reporte/Jugadores?equipo={Uri.EscapeDataString(equipo.Nombre)}";

            foreach (Jugador j in todos)
            {
                if (j.id_Equipo == id_equipo)
                {
                    jugadores.Add(new JugadorDto(j.Nombre, j.Apellido, j.Estatura, j.Posicion, j.Nacionalidad, j.Edad, j.id_Equipo));
                }
            }

            var json = JsonSerializer.Serialize(jugadores);
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


            return File(pdfBytes, "application/pdf", "reporte_jugadores_" + equipo.Nombre + ".pdf");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateJugadorDto>> Get(int id)
        {
            var jugador = await _service.GetByIdAsync(id);
            if (jugador == null)
                return NotFound();

            var dto = new CreateJugadorDto(
                jugador.Nombre,
                jugador.Apellido,
                jugador.Estatura,
                jugador.Posicion,
                jugador.Nacionalidad,
                jugador.Edad,
                jugador.id_Equipo
            );
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateJugadorDto jugador)
        {
            var dto = new Jugador
            {
                Nombre = jugador.Nombre,
                Apellido = jugador.Apellido,
                Nacionalidad = jugador.Nacionalidad,
                Posicion = jugador.posicion,
                Estatura = jugador.estatura,
                Edad = jugador.Edad,
                id_Equipo = jugador.id_Equipo
            };
            var creado = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = creado.id_Jugador }, creado);
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult<JugadorDto>> AsignTeamAsync(int id, [FromBody] JugadorAsignarDto jugadorDto)
        {
            if (id != jugadorDto.id_Jugador)
                return BadRequest("El ID no coincide");

            var jugador = await _service.GetByIdAsync(id);
            if (jugador == null)
                return NotFound();

            jugador.id_Equipo = jugadorDto.Id_Equipo;

            var mapJugador = new Jugador()
            {
                id_Jugador = id,
                Nombre = jugador.Nombre,
                Apellido = jugador.Apellido,
                Posicion = jugador.Posicion,
                Estatura = jugador.Estatura,
                Nacionalidad = jugador.Nacionalidad,
                Edad = jugador.Edad,
                id_Equipo = jugador.id_Equipo
            };

            var actualizado = await _service.UpdateAsync(mapJugador);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var jugador = await _service.GetByIdAsync(id);
            if (jugador == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return Ok("Jugador eliminado");
        }

        [HttpGet("Paginado")]
        public async Task<Pagina<JugadorPaginaDto>> GetLocalidadAsync([FromQuery] int pagina = 1, [FromQuery] int tamanio = 10)
        {
            var todos = await _service.GetAllAsync();
            var jugadores = await _service.GetValuePerPage(pagina, tamanio);
            List<JugadorPaginaDto> jg = new List<JugadorPaginaDto>();

            foreach (Jugador j in jugadores)
            {
                var eq = await _EquipoService.GetByIdAsync(j.id_Equipo);
                jg.Add(new JugadorPaginaDto(
                    j.id_Jugador,
                    j.Nombre,
                    j.Apellido,
                    j.Estatura,        // Orden correcto: tercera posición
                    j.Posicion,        // Cuarta posición
                    j.Nacionalidad,
                    j.Edad,
                    j.id_Equipo
                ));
            }

            return new Pagina<JugadorPaginaDto>
            {
                Items = jg,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(todos.Count() / (double)tamanio),
                TotalRegistros = todos.Count()
            };
        }
        [HttpGet("Reporte/EstadisticasJugador")]
        public async Task<IActionResult> GetEstadisticasJugador([FromQuery] int id_jugador)
        {
            var pythonUrl = $"{_reportServiceBaseUrl.TrimEnd('/')}/Reporte/Estadistica/Jugador";

            var jugador = await _service.GetByIdAsync(id_jugador);
            if (jugador == null)
                return NotFound("Jugador no encontrado");

            var faltas = await _faltas.GetAllAsync();
            var anotaciones = await _anotaciones.GetAllAsync();
            var jugadorFaltas = faltas.Where(f => f.id_jugador == id_jugador).ToList();
            var jugadorAnotaciones = anotaciones.Where(a => a.id_jugador == id_jugador).ToList();

            var payload = new
            {
                jugador = new
                {
                    jugador.id_Jugador,
                    jugador.Nombre,
                    jugador.Apellido,
                    jugador.Posicion,
                    jugador.Edad,
                    jugador.Estatura,
                    jugador.Nacionalidad
                },
                total_faltas = faltas.Select(f => new
                {
                    id_Partido = f.id_partido,
                    total_faltas = f.total_falta
                }),
                total_anotaciones = anotaciones.Select(a => new
                {
                    id_partido = a.id_partido,
                    total_anotaciones = a.total_anotaciones
                })
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(pythonUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                return BadRequest($"Error del servicio Python: {errorMsg}");
            }

            var pdfBytes = await response.Content.ReadAsByteArrayAsync();

            return File(pdfBytes, "application/pdf", $"estadisticas_jugador_{jugador.Nombre}_{jugador.Apellido}.pdf");
        }


    }
}
