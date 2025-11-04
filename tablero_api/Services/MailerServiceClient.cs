using System.Net.Http.Json;
using tablero_api.Services.Interfaces;

namespace tablero_api.Services
{
    public sealed class MailerServiceClient(HttpClient http) : IMailerServiceClient
    {
        public async Task<MailerSendResponse> SendAsync(MailerSendRequest request, CancellationToken ct = default)
        {
            var resp = await http.PostAsJsonAsync("/send-email", request, ct);
            var content = await resp.Content.ReadFromJsonAsync<MailerSendResponse>(cancellationToken: ct)
                          ?? new MailerSendResponse { Success = false, Message = "Respuesta vacía del mailer" };

            return content;
        }
    }
}