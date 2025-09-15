using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tablero_api.Models
{
    public class Usuario
    {
        [Key]
        public int Id_Usuario
        {
            get; set;
        }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Contrasena { get; set; } = string.Empty;
        [ForeignKey(nameof(Rol))]
        public int Id_Rol { get; set; }
        public Rol Rol
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
    }
}
