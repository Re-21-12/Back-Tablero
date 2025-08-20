using System.Text.Json.Serialization;

namespace tablero_api.Models.DTOS
{
    public record CreateTableroDto
    {

        [JsonPropertyOrder(1)]
        public  CreatePartidoDto Partido { get; set; }

        [JsonPropertyOrder(3)]
        public Equipo Local { get; set; } = new Equipo();

        [JsonPropertyOrder(4)]
        public Equipo Visitante { get; set; } = new Equipo();

        [JsonPropertyOrder(5)]
        public Localidad Localidad { get; set; } = new Localidad();

        [JsonPropertyOrder(2)]
        public List<CreateCuartoDto> Cuartos { get; set; } = new List<CreateCuartoDto>();
    }
}
