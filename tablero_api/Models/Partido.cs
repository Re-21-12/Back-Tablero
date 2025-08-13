using tablero_api.Data;

namespace tablero_api.Models
{
    public class Partido
    {
        public int id_Partido { get; set; } 
        public DateTime FechaHora { get; set; }
        public int id_Localidad { get; set; }
        public int id_Local { get; set; }
        public int id_Visitante { get; set; }
        

    }
}
