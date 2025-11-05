namespace tablero_api.Models.DTOS
{
    // ====== Requests ======
    public class SendEmailRequest
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }

    public class SendFromTemplateRequest
    {
        public required string TemplateKey { get; set; }
        public string? Locale { get; set; }
        public required string To { get; set; }
        public Dictionary<string, object>? Data { get; set; }
    }

    // ====== Responses base ======
    public class EmailResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }

    // ====== Tipos "raw" que devuelve el micro en Go (por sql.Null*) ======
    public class GoNullString
    {
        public string? String { get; set; }
        public bool Valid { get; set; }
    }

    public class GoNullTime
    {
        public DateTime Time { get; set; }
        public bool Valid { get; set; }
    }

    public class EmailItemRaw
    {
        public long Id { get; set; }
        public string To { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public string Status { get; set; } = "";
        public GoNullString? Error { get; set; }
        public DateTime CreatedAt { get; set; }
        public GoNullTime? SentAt { get; set; }
    }

    // ====== Modelos "limpios" que expondrá tu API ======
    public class EmailItem
    {
        public long Id { get; set; }
        public string To { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public string Status { get; set; } = "";
        public string? Error { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }

    public class TemplateItem
    {
        public long Id { get; set; }
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TemplateVersionItem
    {
        public long TemplateID { get; set; }
        public int Version { get; set; }
        public string Locale { get; set; } = "es-GT";
        public string Subject { get; set; } = "";
        public string? BodyHtml { get; set; }
        public string? BodyText { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTemplateVersionRequest
    {
        public required string TemplateKey { get; set; }
        public required string Locale { get; set; }
        public required string Subject { get; set; }
        public string? BodyHtml { get; set; }
        public string? BodyText { get; set; }
        public bool Activate { get; set; } = false;
    }

    public class ActivateTemplateVersionRequest
    {
        public required string TemplateKey { get; set; }
        public int Version { get; set; }
        public required string Locale { get; set; }
    }
}
