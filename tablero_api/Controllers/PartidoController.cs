using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using tablero_api.Models;
using tablero_api.Models.DTOS;
using tablero_api.Services;
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
        private readonly IService<Cuarto> _cuartoService;
        private readonly IService<Jugador> _jugadorService;
        private readonly HttpClient _httpClient;


        public PartidoController(IService<Partido> partidoService, IService<Equipo> equipoService, IService<Localidad> localidadSerice, IService<Cuarto> cuartoService, HttpClient httpClient, IService<Jugador> jugadorService)
        {
            _partidoService = partidoService;
            _equipoService = equipoService;
            _localidadService = localidadSerice;
            _cuartoService = cuartoService;
            _jugadorService = jugadorService;
            _httpClient = httpClient;
        }
        [HttpGet("Resultado")]
        public async Task<ActionResult<IEnumerable<PartidoResultadoDto>>> GetPartidosConResultado()
        {
            var partidos = await _partidoService.GetAllAsync();
            var cuartos = await _cuartoService.GetAllAsync();
            var equipos = await _equipoService.GetAllAsync(); // Trae todos de una vez

            var partidoResultados = partidos.Select(p =>
            {
                var cuartosPartido = cuartos.Where(c => c.id_Partido == p.id_Partido);

                int total_local = cuartosPartido
                    .Where(c => c.id_Equipo == p.id_Local)
                    .Sum(c => c.Total_Punteo);

                int total_visitante = cuartosPartido
                    .Where(c => c.id_Equipo == p.id_Visitante)
                    .Sum(c => c.Total_Punteo);

                var local = equipos.FirstOrDefault(e => e.id_Equipo == p.id_Local);
                var visitante = equipos.FirstOrDefault(e => e.id_Equipo == p.id_Visitante);

                return new PartidoResultadoDto(
                    p.id_Partido,
                    local?.Nombre ?? "Desconocido",
                    visitante?.Nombre ?? "Desconocido",
                    
                    new ResultadoDto(p.id_Partido, total_local, total_visitante),
                    fecha: p.FechaHora
                );
            }).ToList();

            return Ok(partidoResultados);
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartidoDto>>> Get()
        {
            var partidos = await _partidoService.GetAllAsync();
            var partidosDto = new List<PartidoDto>();

            foreach (var partido in partidos)
            {
                var equipoLocal = await _equipoService.GetByIdAsync(partido.id_Local);
                var equipoVisitante = await _equipoService.GetByIdAsync(partido.id_Visitante);
                
                partidosDto.Add(new PartidoDto(
                    partido.FechaHora,
                    partido.id_Localidad,
                    partido.id_Local,
                    partido.id_Visitante,
                    equipoLocal?.Nombre ?? "Desconocido",
                    equipoVisitante?.Nombre ?? "Desconocido"
                ));
            }

            return Ok(partidosDto);
        }
        [HttpGet("Paginado")]
        public async Task<ActionResult<Pagina<PartidoDto>>> GetPartidosPerPage([FromQuery] int pagina, [FromQuery] int tamanio)
        {
            var todos = await _partidoService.GetAllAsync();
            var result = await _partidoService.GetValuePerPage(pagina, tamanio);
            var equipos = await _equipoService.GetAllAsync();
            
            List<PartidoDto> dtos = new List<PartidoDto>();
            foreach(Partido p in result)
            {
               
                   
                var visitante = (from e in equipos
                                where e.id_Equipo == p.id_Visitante
                                select e.Nombre)
                                .FirstOrDefault();
                var local = (from e in equipos
                            where e.id_Equipo == p.id_Local
                            select e.Nombre)
                            .FirstOrDefault();

                dtos.Add(new PartidoDto(p.FechaHora, p.id_Localidad, p.id_Local, p.id_Visitante, local.ToString(), visitante.ToString())) ;

            }


            return new Pagina<PartidoDto>
            {
                Items = dtos,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(todos.Count() / (double)tamanio),
                TotalRegistros = todos.Count()
            };
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponsePartidoDto>> Get(int id)
        {
            var partido = await _partidoService.GetByIdAsync(id);
            if (partido == null)
                return NotFound();

            var equipoLocalNombre = await _equipoService.GetByIdAsync(partido.id_Local);
            var equipoVisitanteNombre = await _equipoService.GetByIdAsync(partido.id_Visitante);
            var localidad = await _localidadService.GetByIdAsync(partido.id_Localidad);
            
            var dtoResponse = new ResponsePartidoDto(
                partido.FechaHora,
                localidad?.Nombre ?? "Desconocida",
                equipoLocalNombre?.Nombre ?? "Desconocido",
                equipoVisitanteNombre?.Nombre ?? "Desconocido"
            );
            return Ok(dtoResponse);
        }
        [HttpGet("reporte")]
        public async Task<ActionResult<ReportePartidoDto>> GetReporte()
        {

            string python_string = "http://127.0.0.1:8000/Reporte/Partidos";
            var partidos = await _partidoService.GetAllAsync();
            var cuartos = await _cuartoService.GetAllAsync();
            var equipos = await _equipoService.GetAllAsync(); // Trae todos de una vez

            var partidoResultados = partidos.Select(p =>
            {
                var cuartosPartido = cuartos.Where(c => c.id_Partido == p.id_Partido);

                int total_local = cuartosPartido
                    .Where(c => c.id_Equipo == p.id_Local)
                    .Sum(c => c.Total_Punteo);

                int total_visitante = cuartosPartido
                    .Where(c => c.id_Equipo == p.id_Visitante)
                    .Sum(c => c.Total_Punteo);

                var local = equipos.FirstOrDefault(e => e.id_Equipo == p.id_Local);
                var visitante = equipos.FirstOrDefault(e => e.id_Equipo == p.id_Visitante);

                return new PartidoResultadoDto(
                    p.id_Partido,
                    local?.Nombre ?? "Desconocido",
                    visitante?.Nombre ?? "Desconocido",

                    new ResultadoDto(p.id_Partido, total_local, total_visitante),
                    fecha: p.FechaHora
                );
            }).ToList();
            


           

            var json = JsonSerializer.Serialize(partidoResultados);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(python_string, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                return BadRequest($"Error del servicio: {errorMsg}");
            }
            var pdfBytes = await response.Content.ReadAsByteArrayAsync();


            return File(pdfBytes, "application/pdf", "reporte_Historial_Partidos.pdf");


        }
        


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePartidoDto dto)
        {
            var equipoLocal = await _equipoService.GetByIdAsync(dto.id_Local);
            if (equipoLocal == null)
                return BadRequest("Equipo local no encontrado");
                
            var localidad = await _localidadService.GetByIdAsync(equipoLocal.id_Localidad);
            if (localidad == null)
                return BadRequest("Localidad no encontrada");

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdatePartidoDto partidoDto)
        {
            var partido = await _partidoService.GetByIdAsync(id);
            if (partido == null)
                return BadRequest("ID no encontrado");

            // Actualizar las propiedades del partido existente
            partido.FechaHora = partidoDto.FechaHora;
            partido.id_Local = partidoDto.id_Local;
            partido.id_Visitante = partidoDto.id_visitante;

            // Obtener la localidad del equipo local actualizado
            var equipoLocal = await _equipoService.GetByIdAsync(partidoDto.id_Local);
            if (equipoLocal != null)
            {
                partido.id_Localidad = equipoLocal.id_Localidad;
            }

            var actualizado = await _partidoService.UpdateAsync(partido);
            return Ok("Partido Actualizado");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var partido = await _partidoService.GetByIdAsync(id);
            if (partido == null)
                return NotFound();

            await _partidoService.DeleteAsync(id);
            return Ok("Partido eliminado");
        }

        [HttpGet("Reporte/Partido")]
        public async Task<IActionResult> GetReportePartido([FromQuery] int id_partido)
        {
            
            var partido = await _partidoService.GetByIdAsync(id_partido);
            var equipo_local = await _equipoService.GetByIdAsync(partido.id_Local);
            var equipo_visitante = await _equipoService.GetByIdAsync(partido.id_Visitante);
            var localidad = await _localidadService.GetByIdAsync(partido.id_Localidad);
            var jugadores = await _jugadorService.GetAllAsync();
            var jugadores_local = new List<JugadorDto>();
            var jugadores_visitante = new List<JugadorDto>() ;

            foreach (Jugador j in jugadores)
            {
                if (j.id_Equipo == equipo_local.id_Equipo)
                {
                    jugadores_local.Add( new JugadorDto(j.Nombre, j.Apellido, j.Estatura, j.Posicion, j.Nacionalidad, j.Edad, j.id_Equipo));
                }
                if(j.id_Equipo == equipo_visitante.id_Equipo)
                {
                    jugadores_visitante.Add(new JugadorDto(j.Nombre, j.Apellido, j.Estatura, j.Posicion, j.Nacionalidad, j.Edad, j.id_Equipo));
                }
            }
            var jl = jugadores_local.ToString;
            var jv = jugadores_visitante.ToString;
            var body = new
            {
                partido_info = new
                {
                    
                    partido.id_Local,
                    partido.id_Visitante,
                    partido.FechaHora,
                    localidad.Nombre
                },
                equipo_local = equipo_local.Nombre,
                equipo_visitante = equipo_visitante.Nombre,
                jugadores_locales = jugadores_local,
                jugadores_visitantes = jugadores_visitante
            };
            
            var json = JsonSerializer.Serialize(body);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine(json);

            var response = await _httpClient.PostAsync("http://127.0.0.1:8000/Reporte/Partido/Roster", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                return BadRequest($"Error del servicio Python: {errorMsg}");
            }

            var pdfBytes = await response.Content.ReadAsByteArrayAsync();
            return File(pdfBytes, "application/pdf", $"reporte_partido_{equipo_local.Nombre}_vs_{equipo_visitante.Nombre}.pdf");
        }

    }
}