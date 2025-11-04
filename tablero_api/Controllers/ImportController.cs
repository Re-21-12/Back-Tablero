using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using tablero_api.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;
        private readonly ILogger<ImportController> _logger;

        public ImportController(IImportService importService, ILogger<ImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        [HttpPost("equipo/csv")]
        public async Task<IActionResult> ImportEquiposCSV([FromForm] IFormFile file)
            => await HandleFileImport(file, true, "equipo");

        [HttpPost("equipo/json")]
        public async Task<IActionResult> ImportEquiposJSON([FromForm] IFormFile file)
            => await HandleFileImport(file, false, "equipo");

        [HttpPost("jugador/csv")]
        public async Task<IActionResult> ImportJugadoresCSV([FromForm] IFormFile file)
            => await HandleFileImport(file, true, "jugador");

        [HttpPost("jugador/json")]
        public async Task<IActionResult> ImportJugadoresJSON([FromForm] IFormFile file)
            => await HandleFileImport(file, false, "jugador");

        [HttpPost("localidad/csv")]
        public async Task<IActionResult> ImportLocalidadesCSV([FromForm] IFormFile file)
            => await HandleFileImport(file, true, "localidad");

        [HttpPost("localidad/json")]
        public async Task<IActionResult> ImportLocalidadesJSON([FromForm] IFormFile file)
            => await HandleFileImport(file, false, "localidad");

        [HttpPost("partido/csv")]
        public async Task<IActionResult> ImportPartidosCSV([FromForm] IFormFile file)
            => await HandleFileImport(file, true, "partido");

        [HttpPost("partido/json")]
        public async Task<IActionResult> ImportPartidosJSON([FromForm] IFormFile file)
            => await HandleFileImport(file, false, "partido");

        private async Task<IActionResult> HandleFileImport(IFormFile file, bool isCsv, string tipo)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ImportResponse(0, 1, new System.Collections.Generic.List<string> { "El archivo está vacío" }));
            }

            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();

                ImportResponse resp = tipo switch
                {
                    "equipo" => await _importService.ImportEquiposAsync(bytes, isCsv, file.FileName),
                    "jugador" => await _importService.ImportJugadoresAsync(bytes, isCsv, file.FileName),
                    "localidad" => await _importService.ImportLocalidadesAsync(bytes, isCsv, file.FileName),
                    "partido" => await _importService.ImportPartidosAsync(bytes, isCsv, file.FileName),
                    _ => throw new ArgumentException("Tipo no válido")
                };

                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error procesando importación");
                return BadRequest(new ImportResponse(0, 1, new System.Collections.Generic.List<string> { "Error procesando archivo: " + ex.Message }));
            }
        }

        [HttpGet("health")]
        public IActionResult Health() => Ok("Microservicio funcionando correctamente");
    }
}
