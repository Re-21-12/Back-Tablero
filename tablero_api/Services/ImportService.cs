using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using tablero_api.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Services
{
    public class ImportService : IImportService
    {
        private readonly HttpClient _client;
        private readonly ILogger<ImportService> _logger;

        public ImportService(HttpClient client, ILogger<ImportService> logger, IConfiguration config)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger;

            // If BaseAddress wasn't configured via DI, use a fallback from configuration
            if (_client.BaseAddress == null)
            {
                var baseUrl = config.GetValue<string>("MicroServices:ImportService", "http://import-service:8080");
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    _client.BaseAddress = new Uri(baseUrl);
                }
            }
        }

        public Task<ImportResponse> ImportEquiposAsync(byte[] fileBytes, bool isCsv, string? fileName = null)
            => SendFileToImportService("equipo", fileBytes, isCsv, fileName);

        public Task<ImportResponse> ImportJugadoresAsync(byte[] fileBytes, bool isCsv, string? fileName = null)
            => SendFileToImportService("jugador", fileBytes, isCsv, fileName);

        public Task<ImportResponse> ImportLocalidadesAsync(byte[] fileBytes, bool isCsv, string? fileName = null)
            => SendFileToImportService("localidad", fileBytes, isCsv, fileName);

        public Task<ImportResponse> ImportPartidosAsync(byte[] fileBytes, bool isCsv, string? fileName = null)
            => SendFileToImportService("partido", fileBytes, isCsv, fileName);

        private async Task<ImportResponse> SendFileToImportService(string tipo, byte[] fileBytes, bool isCsv, string? fileName)
        {
            try
            {
                var format = isCsv ? "csv" : "json";
                var requestUri = $"/import/{tipo}/{format}";

                using var content = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(isCsv ? "text/csv" : "application/json");
                var name = string.IsNullOrEmpty(fileName) ? $"{tipo}.{format}" : fileName;
                content.Add(fileContent, "file", name);

                var response = await _client.PostAsync(requestUri, content);
                var respString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    _logger?.LogWarning("Import service returned {Status}: {Body}", response.StatusCode, respString);
                    return new ImportResponse(0, 1, new System.Collections.Generic.List<string> { $"Import service error: {response.StatusCode}" });
                }

                var importResp = JsonSerializer.Deserialize<ImportResponse>(respString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return importResp ?? new ImportResponse(0, 1, new System.Collections.Generic.List<string> { "Respuesta inválida del import-service" });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error enviando archivo al import-service");
                return new ImportResponse(0, 1, new System.Collections.Generic.List<string> { "Error enviando archivo al import-service: " + ex.Message });
            }
        }
    }
}
