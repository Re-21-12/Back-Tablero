using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using tablero_api.Migrations;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JugadorController : ControllerBase
    {
        private readonly IService<Jugador> _service;
        private readonly IService<Equipo> _EquipoService;
        private readonly HttpClient _httpClient;

        public JugadorController(IService<Jugador> service, IService<Equipo> equipoService, HttpClient httpClient)
        {
            _service = service;
            _EquipoService = equipoService;
            _httpClient = httpClient;
        }
        [HttpGet("byTeam/{id_equipo}")]
        public async Task<ActionResult<IEnumerable<JugadorDto>>> GetByTeam(int id_equipo)
        {
            var equipo = await _EquipoService.GetByIdAsync(id_equipo);
            var jugadores = await _service.GetAllAsync();
            List<JugadorDto> ret = new List<JugadorDto>();
            foreach(Jugador j in jugadores){
                if(j.id_Equipo == id_equipo)
                {
                    ret.Add(new JugadorDto(j.Nombre, j.Apellido, j.Estatura, j.Posicion, j.Nacionalidad, j.Edad, j.id_Equipo));
                }
            }


            if (equipo == null)
                return NotFound("No existe el equipo");
            return Ok(ret);
        }
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
            string python_string = "http://127.0.0.1:8000/Reporte/Jugadores";
            var todos = await _service.GetAllAsync();
            var equipo = await _EquipoService.GetByIdAsync(id_equipo);
            var jugadores = new List<JugadorDto>();
            python_string = python_string+ "?equipo=" + equipo.Nombre;


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


            return File(pdfBytes, "application/pdf", "reporte_jugadores_"+equipo.Nombre+".pdf");
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
        
    }
}
