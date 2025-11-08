using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace tablero_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImportController> _logger;
        private readonly string? _importServiceBaseUrl;

        public ImportController(IHttpClientFactory httpClientFactory, ILogger<ImportController> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _importServiceBaseUrl = configuration.GetValue<string>("MicroServices:ImportService");
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
                return BadRequest(new { message = "El archivo está vacío" });

            try
            {
                var client = _httpClientFactory.CreateClient("ImportService");

                string requestUri;
                if (client.BaseAddress != null)
                {
                    // Relative path when BaseAddress configured
                    requestUri = $"/import/{tipo}/{(isCsv ? "csv" : "json")}";
                }
                else if (!string.IsNullOrWhiteSpace(_importServiceBaseUrl))
                {
                    requestUri = $"{_importServiceBaseUrl.TrimEnd('/')}/import/{tipo}/{(isCsv ? "csv" : "json")}";
                }
                else
                {
                    requestUri = $"http://import-service:8080/import/{tipo}/{(isCsv ? "csv" : "json")}";
                }

                using var content = new MultipartFormDataContent();
                await using var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                // Match Spring Boot controller field name "file"
                content.Add(fileContent, "file", file.FileName);

                // Optional: set timeout per-request if needed by using CancellationTokenSource
                var response = await client.PostAsync(requestUri, content);

                var responseBody = await response.Content.ReadAsStringAsync();

                // Preserve status code and content type (default to application/json)
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";

                return new ContentResult
                {
                    StatusCode = (int)response.StatusCode,
                    Content = responseBody,
                    ContentType = contentType
                };
            }
            catch (HttpRequestException hre)
            {
                _logger?.LogError(hre, "HTTP error proxying import request");
                return StatusCode((int)HttpStatusCode.BadGateway, new { message = "Error de comunicación con el servicio de importación", detail = hre.Message });
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