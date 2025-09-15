using System.ComponentModel.DataAnnotations;

namespace tablero_api.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id
        {
            get; set;
        }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate
        {
            get; set;
        }
        public int UsuarioId
        {
            get; set;
        }
        public Usuario Usuario { get; set; } = null!;
        public bool IsRevoked { get; set; } = false;
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
