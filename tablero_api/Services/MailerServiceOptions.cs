namespace tablero_api.Services
{
    public class MailerServiceOptions
    {
        public string BaseUrl { get; set; } = string.Empty; 
        public int TimeoutSeconds { get; set; } = 30;
    }
}
