namespace tablero_api.Models
{
    public class Cuarto
    {
        public int id_Cuarto { get; set; }
        public int No_Cuarto { get; set; }
        public int Total_Punteo { get; set; }
        public int Total_Faltas { get; set; }


        // FK a Partido
        public int id_Partido { get; set; }
        public Partido Partido { get; set; } = null!;

        public int id_Equipo { get; set; }
        public Equipo Equipo { get; set; }
    }
}
