using System;
using System.ComponentModel.DataAnnotations;

namespace AkilliYemekTarifOneriSistemi.Models
{
    public class SupportMessage
    {
        public int Id { get; set; }

        [Required]
        public int ThreadId { get; set; }          

        public SupportThread? Thread { get; set; } 

        [Required]
        public string UserId { get; set; } = string.Empty;

        
        public string? UserEmail { get; set; }

        [Required]
        public string SenderRole { get; set; } = "User"; 

        [Required, MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
