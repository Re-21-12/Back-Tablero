using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using tablero_api.DTOs;

namespace tablero_api.Services
{
    public class MailerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public MailerService(IConfiguration config, IHttpClientFactory clientFactory)
        {
            // Intentamos usar el cliente nombrado "MailerService" si fue registrado en Program.cs;
            // si no existe, CreateClient("MailerService") fallará y el factory devolverá un cliente por defecto.
            try
            {
                _httpClient = clientFactory.CreateClient("MailerService");
            }
            catch
            {
                _httpClient = clientFactory.CreateClient();
            }

            _baseUrl = config["MailerService:BaseUrl"]?.TrimEnd('/') ?? "http://localhost:8080";
        }

        // ============================================================
        // ENVIAR CORREO
        // ============================================================
        public async Task<object?> SendEmailAsync(SendEmailDto dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/send", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Microservicio devolvió {response.StatusCode}: {body}");

            return JsonSerializer.Deserialize<object?>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ============================================================
        // LISTAR TODOS LOS CORREOS 
        // ============================================================
        public async Task<object?> GetAllEmailsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/emails");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Microservicio devolvió {response.StatusCode}: {body}");

            return JsonSerializer.Deserialize<object?>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ============================================================
        //  LISTAR CORREOS CON PAGINACIÓN
        // ============================================================
        public async Task<PaginatedResult<object>> GetPaginatedEmailsAsync(int page, int pageSize)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/emails?page={page}&pageSize={pageSize}");
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Microservicio devolvió {response.StatusCode}: {json}");

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Leer propiedades del JSON real
                var currentPage = root.TryGetProperty("page", out var pageProp) ? pageProp.GetInt32() : 1;
                var currentPageSize = root.TryGetProperty("pageSize", out var sizeProp) ? sizeProp.GetInt32() : 10;
                var total = root.TryGetProperty("total", out var totalProp) ? totalProp.GetInt32() : 0;
                var items = root.TryGetProperty("data", out var dataProp)
                    ? JsonSerializer.Deserialize<IEnumerable<object>>(dataProp.GetRawText())
                    : new List<object>();

                return new PaginatedResult<object>
                {
                    Page = currentPage,
                    PageSize = currentPageSize,
                    TotalCount = total,
                    Items = items ?? new List<object>()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error procesando JSON del microservicio: {ex.Message}\nContenido: {json}");
            }
        }


        // ============================================================
        // OBTENER CORREO POR ID
        // ============================================================
        public async Task<object?> GetEmailByIdAsync(long id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/emails/{id}");
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Microservicio devolvió {response.StatusCode}: {json}");

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("data", out var dataElement))
                {
                    return JsonSerializer.Deserialize<object>(
                        dataElement.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }

                return JsonSerializer.Deserialize<object>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error procesando JSON del microservicio: {ex.Message}\nContenido: {json}");
            }
        }

        // ============================================================
        //  ELIMINAR CORREO
        // ============================================================
        public async Task<bool> DeleteEmailAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/emails/{id}");
            return response.IsSuccessStatusCode;
        }

        // ============================================================
        // CREAR PLANTILLA
        // ============================================================
        public async Task<object?> CreateTemplateAsync(MailTemplateDto dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/templates", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Microservicio devolvió {response.StatusCode}: {body}");

            return JsonSerializer.Deserialize<object?>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ============================================================
        // ACTUALIZAR PLANTILLA
        // ============================================================
        public async Task<object?> UpdateTemplateAsync(int id, MailTemplateDto dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/templates/{id}", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Microservicio devolvió {response.StatusCode}: {body}");

            return JsonSerializer.Deserialize<object?>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ============================================================
        //  ELIMINAR PLANTILLA
        // ============================================================
        public async Task<bool> DeleteTemplateAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/templates/{id}");
            return response.IsSuccessStatusCode;
        }


        public async Task<object?> GetAllTemplatesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/templates");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Microservicio devolvió {response.StatusCode}: {body}");

            return JsonSerializer.Deserialize<object?>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ============================================================
        //  MODELO DE RESULTADO PAGINADO
        // ============================================================
        public class PaginatedResult<T>
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public IEnumerable<T> Items { get; set; } = new List<T>();
        }
    }
}
