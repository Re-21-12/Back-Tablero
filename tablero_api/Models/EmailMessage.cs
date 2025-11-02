namespace tablero_api.Models
{
    public enum EmailStatus { Draft = 0, Sent = 1, Failed = 2 }

    public class EmailMessage
    {
        public int Id { get; set; }

        public string To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;

        public EmailStatus Status { get; set; } = EmailStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }

        public string? Error { get; set; }
    }
}