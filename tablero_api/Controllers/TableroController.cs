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
    public class TableroController : ControllerBase
    {
        private readonly IService<Partido> _partidoService;
        private readonly IService<Equipo> _equipoService;
        private readonly IService<Localidad> _localidadService;
        private readonly IService<Cuarto> _cuartoService;

        public TableroController(IService<Partido> partidoService, IService<Equipo> equipoService, IService<Localidad> localidadService, IService<Cuarto> cuartoService)
        {
            _partidoService = partidoService;
            _equipoService = equipoService;
            _localidadService = localidadService;
            _cuartoService = cuartoService;
        }

        [HttpGet("{id}/resultado")]
        public async Task<ActionResult<ResultadoPartidoDto>> ResultadoPartido(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("El ID del partido debe ser un número positivo.");
                }

                var partido = await _partidoService.GetByIdAsync(id);
                var equipoLocal = partido != null ? await _equipoService.GetByIdAsync(partido.id_Local) : null;
                if(equipoLocal == null)
                {
                    return NotFound("Equipo local no encontrado.");
                }
                var equipoVisitante = partido != null ? await _equipoService.GetByIdAsync(partido.id_Visitante) : null;

                if (partido == null || equipoLocal == null || equipoVisitante == null)
                {
                    return NotFound("Partido o equipos no encontrados.");
                }

                var cuartosEquipoLocal = await _cuartoService.GetByTwoParameters(partido.id_Partido, equipoLocal.id_Equipo);
                var cuartosEquipoVisitante = await _cuartoService.GetByTwoParameters(partido.id_Partido, equipoVisitante.id_Equipo);

                var totalPunteoLocal = cuartosEquipoLocal.Sum(c => c.Total_Punteo);
                var totalPunteoVisitante = cuartosEquipoVisitante.Sum(c => c.Total_Punteo);
                var totalFaltasLocal = cuartosEquipoLocal.Sum(c => c.Total_Faltas);
                var totalFaltasVisitante = cuartosEquipoVisitante.Sum(c => c.Total_Faltas);

                var localidad = await _localidadService.GetByIdAsync(equipoLocal.id_Localidad);

                var resultado = new ResultadoPartidoDto(
                    equipoLocal.Nombre,
                    equipoVisitante.Nombre,
                    localidad?.Nombre ?? "Desconocida",
                    totalFaltasLocal,
                    totalFaltasVisitante,
                    totalPunteoLocal,
                    totalPunteoVisitante
                );

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}