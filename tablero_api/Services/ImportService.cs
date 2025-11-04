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

                // Intento de mapear la respuesta de forma tolerante: el import-service puede usar
                // nombres distintos y `errors` puede ser número o array. No fallaremos por eso.
                try
                {
                    using var doc = JsonDocument.Parse(respString);
                    var root = doc.RootElement;

                    int processed = 0;
                    int errorsCount = 0;
                    var messages = new System.Collections.Generic.List<string>();

                    // Procesed / Imported / processed
                    if (TryGetInt(root, new[] { "processed", "imported", "processedCount", "processed_count" }, out var p))
                        processed = p;

                    // Errors can be an int or an array or a string
                    if (TryGetProperty(root, new[] { "errors", "failed", "errorCount", "errors_count" }, out var errElem))
                    {
                        switch (errElem.ValueKind)
                        {
                            case JsonValueKind.Number:
                                if (errElem.TryGetInt32(out var n)) errorsCount = n;
                                break;
                            case JsonValueKind.Array:
                                foreach (var item in errElem.EnumerateArray())
                                {
                                    messages.Add(item.ToString().Trim('"'));
                                }
                                errorsCount = messages.Count;
                                break;
                            case JsonValueKind.String:
                                var s = errElem.GetString();
                                if (!string.IsNullOrEmpty(s)) messages.Add(s);
                                errorsCount = messages.Count > 0 ? messages.Count : 0;
                                break;
                            case JsonValueKind.Object:
                                messages.Add(errElem.GetRawText());
                                errorsCount = 1;
                                break;
                        }
                    }

                    // Messages / messages
                    if (TryGetProperty(root, new[] { "messages", "mensajes", "messages_list" }, out var msgElem))
                    {
                        if (msgElem.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in msgElem.EnumerateArray())
                                messages.Add(item.ToString().Trim('"'));
                        }
                        else if (msgElem.ValueKind == JsonValueKind.String)
                        {
                            var s2 = msgElem.GetString();
                            if (!string.IsNullOrEmpty(s2)) messages.Add(s2);
                        }
                    }

                    // Si no tenemos messages pero la respuesta es un string o array, guardarla como mensaje
                    if (messages.Count == 0)
                    {
                        if (root.ValueKind == JsonValueKind.String)
                        {
                            var s3 = root.GetString();
                            if (!string.IsNullOrEmpty(s3)) messages.Add(s3);
                        }
                    }

                    return new ImportResponse(processed, errorsCount, messages);
                }
                catch (JsonException)
                {
                    // Fallback: devolver el cuerpo tal cual en Messages si no es JSON esperado
                    return new ImportResponse(0, 0, new System.Collections.Generic.List<string> { respString });
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error enviando archivo al import-service");
                return new ImportResponse(0, 1, new System.Collections.Generic.List<string> { "Error enviando archivo al import-service: " + ex.Message });
            }
        }

        // Helpers locales
        private static bool TryGetProperty(JsonElement root, string[] names, out JsonElement element)
        {
            foreach (var n in names)
            {
                if (root.TryGetProperty(n, out element)) return true;
            }
            element = default;
            return false;
        }

        private static bool TryGetInt(JsonElement root, string[] names, out int value)
        {
            value = 0;
            if (TryGetProperty(root, names, out var elem))
            {
                if (elem.ValueKind == JsonValueKind.Number && elem.TryGetInt32(out var v))
                {
                    value = v;
                    return true;
                }
            }
            return false;
        }
    }
}
