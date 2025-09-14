using System.ComponentModel.DataAnnotations;

namespace tablero_api.Models
{
    public class Rol
    {
        [Key]
        public int Id_Rol
        {
            get; set;
        }
        [Required]
        public required string Nombre
        {
            get; set;
        }
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
        public ICollection<Usuario> Usuarios { get; set; } = [];
        public ICollection<Permiso> Permisos { get; set; } = [];
    }
}
