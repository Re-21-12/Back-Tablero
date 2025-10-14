using System.ComponentModel.DataAnnotations;
namespace tablero_api.Models
{
    public class Permiso
    {
        [Key]
        public int Id_Permiso
        {
            get; set;
        }
        public int Id_Rol
        {
            get; set;
        }
        public Rol Rol{ get; set; }
        public string Nombre { get; set; } = string.Empty;
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