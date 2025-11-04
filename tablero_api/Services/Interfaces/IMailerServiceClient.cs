<<<<<<< HEAD
using System.Net.Http.Json;
=======
﻿using System.Net.Http.Json;
>>>>>>> upstream/stableDani

namespace tablero_api.Services.Interfaces
{
    public interface IMailerServiceClient
    {
        Task<MailerSendResponse> SendAsync(MailerSendRequest request, CancellationToken ct = default);
    }

    public sealed class MailerSendRequest
    {
        public string To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
    }

    public sealed class MailerSendResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> upstream/stableDani
