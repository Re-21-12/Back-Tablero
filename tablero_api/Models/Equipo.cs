namespace tablero_api.Models
{
    public class Equipo
    {
        public int id_Equipo { get; set; }
        public string Nombre { get; set; }
        public int id_Localidad { get; set; }
        public Localidad Localidad { get; set; }
    }
}
