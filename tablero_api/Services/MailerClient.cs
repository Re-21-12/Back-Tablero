
using System.Text.Json.Serialization;
using tablero_api.Models.DTOS;
using tablero_api.Services.Interfaces;

namespace tablero_api.Services
{
    public class MailerClient : IMailerClient
    {
        private readonly HttpClient _http;
        public MailerClient(HttpClient http) => _http = http;

        // === DTOs crudos (por sql.Null*) que devuelve el micro Go ===
        private sealed class NullStringDto
        {
            [JsonPropertyName("String")] public string? String { get; set; }
            [JsonPropertyName("Valid")] public bool Valid { get; set; }
        }
        private sealed class NullTimeDto
        {
            [JsonPropertyName("Time")] public DateTime Time { get; set; }
            [JsonPropertyName("Valid")] public bool Valid { get; set; }
        }
        private sealed class EmailItemRaw
        {
            [JsonPropertyName("ID")] public long Id { get; set; }
            [JsonPropertyName("To")] public string To { get; set; } = "";
            [JsonPropertyName("Subject")] public string Subject { get; set; } = "";
            [JsonPropertyName("Body")] public string Body { get; set; } = "";
            [JsonPropertyName("Status")] public string Status { get; set; } = "";
            [JsonPropertyName("Error")] public NullStringDto? Error { get; set; }
            [JsonPropertyName("CreatedAt")] public DateTime CreatedAt { get; set; }
            [JsonPropertyName("SentAt")] public NullTimeDto? SentAt { get; set; }
        }
        private static EmailItem Map(EmailItemRaw r) => new EmailItem
        {
            Id = r.Id,
            To = r.To,
            Subject = r.Subject,
            Body = r.Body,
            Status = r.Status,
            Error = (r.Error != null && r.Error.Valid) ? r.Error.String : null,
            CreatedAt = r.CreatedAt,
            SentAt = (r.SentAt != null && r.SentAt.Valid) ? r.SentAt.Time : (DateTime?)null
        };

        // === Envío directo ===
        public async Task<EmailResponse> SendAsync(SendEmailRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("/send", req, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<EmailResponse>(cancellationToken: ct)
                   ?? new EmailResponse { Success = false, Message = "Respuesta vacía" };
        }

        // === Borradores ===
        private sealed class DraftCreateResp
        {
            public bool Success { get; set; }
            public long Id { get; set; }
            public string? Error { get; set; }
        }
        public async Task<long> CreateDraftAsync(SendEmailRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("/drafts", req, ct);
            resp.EnsureSuccessStatusCode();
            var dto = await resp.Content.ReadFromJsonAsync<DraftCreateResp>(cancellationToken: ct)
                      ?? new DraftCreateResp { Success = false };
            if (!dto.Success) throw new InvalidOperationException(dto.Error ?? "No se pudo crear borrador");
            return dto.Id;
        }
        public async Task<bool> UpdateDraftAsync(long id, SendEmailRequest req, CancellationToken ct = default)
        {
            var resp = await _http.PutAsJsonAsync($"/drafts/{id}", req, ct);
            return resp.IsSuccessStatusCode;
        }

        // === Lecturas / Admin ===
        public async Task<List<EmailItem>> ListEmailsAsync(string? status = null, int limit = 50, int offset = 0, CancellationToken ct = default)
        {
            var url = $"/emails?limit={limit}&offset={offset}" +
                      (string.IsNullOrWhiteSpace(status) ? "" : $"&status={Uri.EscapeDataString(status)}");
            var raw = await _http.GetFromJsonAsync<List<EmailItemRaw>>(url, ct) ?? new();
            return raw.Select(Map).ToList();
        }
        public async Task<EmailItem?> GetEmailByIdAsync(long id, CancellationToken ct = default)
        {
            var raw = await _http.GetFromJsonAsync<EmailItemRaw>($"/emails/{id}", ct);
            return raw is null ? null : Map(raw);
        }
        public async Task<bool> DeleteEmailAsync(long id, CancellationToken ct = default)
        {
            var resp = await _http.DeleteAsync($"/emails/{id}", ct);
            if (!resp.IsSuccessStatusCode) return false;
            var dyn = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>(cancellationToken: ct);
            return dyn != null && dyn.TryGetValue("success", out var v) && v is bool b && b;
        }
    }
}
