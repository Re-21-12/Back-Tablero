using System.Collections.Generic;

namespace tablero_api.DTOS
{
    public class ImportResponse
    {
        public int Processed { get; set; }
        public int Errors { get; set; }
        public List<string> Messages { get; set; }

        public ImportResponse()
        {
            Messages = new List<string>();
        }

        public ImportResponse(int processed, int errors, List<string> messages)
        {
            Processed = processed;
            Errors = errors;
            Messages = messages ?? new List<string>();
        }
    }
}
