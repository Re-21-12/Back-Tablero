namespace tablero_api.Models
{
    public class Equipo
    {
        public int id_Equipo
        {
            get; set;
        }
        public string Nombre { get; set; } = string.Empty;
        public int id_Localidad
        {
            get; set;
        }
        public Localidad Localidad { get; set; } = null!;
        public ICollection<Jugador> Jugadores { get; set; } = [];

    }
}
