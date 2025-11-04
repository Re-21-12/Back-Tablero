using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace tablero_api.Controllers
{
    [ApiController]
    [Route("import")]
    public class ImportController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImportController> _logger;
        private readonly string _importServiceBaseUrl;

        public ImportController(IHttpClientFactory httpClientFactory, ILogger<ImportController> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _importServiceBaseUrl = configuration["ImportService:BaseUrl"] ?? throw new ArgumentNullException("ImportService:BaseUrl");
        }

        [HttpPost("equipo/csv")]
        public async Task<IActionResult> ImportEquiposCSV([FromForm] IFormFile file)
            => await HandleFileProxy(file, true, "equipo");

        [HttpPost("equipo/json")]
        public async Task<IActionResult> ImportEquiposJSON([FromForm] IFormFile file)
            => await HandleFileProxy(file, false, "equipo");

        [HttpPost("jugador/csv")]
        public async Task<IActionResult> ImportJugadoresCSV([FromForm] IFormFile file)
            => await HandleFileProxy(file, true, "jugador");

        [HttpPost("jugador/json")]
        public async Task<IActionResult> ImportJugadoresJSON([FromForm] IFormFile file)
            => await HandleFileProxy(file, false, "jugador");

        [HttpPost("localidad/csv")]
        public async Task<IActionResult> ImportLocalidadesCSV([FromForm] IFormFile file)
            => await HandleFileProxy(file, true, "localidad");

        [HttpPost("localidad/json")]
        public async Task<IActionResult> ImportLocalidadesJSON([FromForm] IFormFile file)
            => await HandleFileProxy(file, false, "localidad");

        [HttpPost("partido/csv")]
        public async Task<IActionResult> ImportPartidosCSV([FromForm] IFormFile file)
            => await HandleFileProxy(file, true, "partido");

        [HttpPost("partido/json")]
        public async Task<IActionResult> ImportPartidosJSON([FromForm] IFormFile file)
            => await HandleFileProxy(file, false, "partido");

        private async Task<IActionResult> HandleFileProxy(IFormFile file, bool isCsv, string tipo)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "El archivo está vacío" });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var targetPath = $"{_importServiceBaseUrl.TrimEnd('/')}/import/{tipo}/{(isCsv ? "csv" : "json")}";

                using var content = new MultipartFormDataContent();
                using var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                content.Add(fileContent, "file", file.FileName);

                var response = await client.PostAsync(targetPath, content);

                var responseBody = await response.Content.ReadAsStringAsync();

                // Reenviar código y cuerpo tal cual (asumiendo JSON). Si quieres, mapear a DTO antes.
                return new ContentResult
                {
                    StatusCode = (int)response.StatusCode,
                    Content = responseBody,
                    ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json"
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error reenviando importación");
                return BadRequest(new { message = "Error procesando archivo: " + ex.Message });
            }
        }

        [HttpGet("health")]
        public IActionResult Health() => Ok("Gateway funcionando correctamente");
    }
}
