using System;
using System.ComponentModel.DataAnnotations;

namespace tablero_api.Models
{
        public class EmailTemplate
        {
                [Key] public Guid Id { get; set; } = Guid.NewGuid();

                [Required, MaxLength(120)]
                public string Name { get; set; } = string.Empty;

                [Required, MaxLength(200)]
                public string Subject { get; set; } = string.Empty;

                [Required]
                public string BodyHtml { get; set; } = string.Empty;

                public bool IsActive { get; set; } = true;

                public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
                public DateTime? UpdatedAt { get; set; }
        }
}
