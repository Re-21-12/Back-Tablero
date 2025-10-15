namespace tablero_api.Models
{
    public class Falta
    {
        public int id { get; set; }
        public int total_falta { get; set; }
        public int id_jugador { get; set; }
        public Jugador jugador { get; set; }
        public int id_cuarto { get; set; }
        public Cuarto cuarto { get; set; }
        public int id_partido { get; set; }
        public Partido partido { get; set; }

    }
}
