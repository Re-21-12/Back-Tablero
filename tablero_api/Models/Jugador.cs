using System.ComponentModel.DataAnnotations;

namespace tablero_api.Models
{
    public class Jugador
    {
        [Key]
        public int id_Jugador
        {
            get; set;
        }
        public string Nombre { get; set; } = string.Empty;
        public int Edad
        {
            get; set;
        }

        public int id_Equipo
        {
            get; set;
        }

        public Equipo Equipo{ get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CreatedBy
        {
            get; set;
        }
        public DateTime? UpdatedAt
        {
            get; set;
        }
        public int UpdatedBy
        {
            get; set;
        }
    }
}
