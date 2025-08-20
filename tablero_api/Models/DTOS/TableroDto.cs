namespace tablero_api.Models.DTOS
{
    public record CreateTableroDto
    {
        public List<Cuarto> Cuartos { get; set; } = new List<Cuarto>();
        public Partido Partido { get; set; } = new Partido();
        public Equipo Local { get; set; } = new Equipo();
        public Equipo Visitante { get; set; } = new Equipo();
        public Localidad Localidad { get; set; } = new Localidad();
    }
}
