<<<<<<< HEAD
namespace tablero_api.Models.DTOS
=======
﻿namespace tablero_api.Models.DTOS
>>>>>>> upstream/stableDani
{
    public sealed class EmailRequestDto
    {
        public string To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
    }
}