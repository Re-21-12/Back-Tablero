namespace tablero_api.Services.Interfaces
{
    using tablero_api.Models.DTOS;

    public interface IMailerClient
    {
        // Envío directo
        Task<EmailResponse> SendAsync(SendEmailRequest req, CancellationToken ct = default);

        // Borradores
        Task<long> CreateDraftAsync(SendEmailRequest req, CancellationToken ct = default);
        Task<bool> UpdateDraftAsync(long id, SendEmailRequest req, CancellationToken ct = default);

        // Lecturas / Admin
        Task<List<EmailItem>> ListEmailsAsync(string? status = null, int limit = 50, int offset = 0, CancellationToken ct = default);
        Task<EmailItem?> GetEmailByIdAsync(long id, CancellationToken ct = default);
        Task<bool> DeleteEmailAsync(long id, CancellationToken ct = default);
    }
}