namespace tablero_api.Models
{
    public class KeycloakOptions
    {
        public string Authority { get; set; } = string.Empty; // e.g. http://host:8080/realms/tablero
        public string ClientId { get; set; } = string.Empty;
        public bool RequireHttpsMetadata { get; set; } = false;
        public string? ClientSecret { get; set; }
        public string? AdminClientId { get; set; }
    }
}