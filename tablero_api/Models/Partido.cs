using tablero_api.Data;

namespace tablero_api.Models
{
    public class Partido
    {
        public int id_Partido { get; set; } // PK
        public DateTime FechaHora { get; set; }

        // Relaciones
        public int id_Localidad { get; set; }
        public Localidad Localidad { get; set; } = null!;

        public int id_Local { get; set; }
        public Equipo local { get; set; }
        public int id_Visitante { get; set; }
        public Equipo Visitante { get; set; }
    }
}
