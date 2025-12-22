using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AkilliYemekTarifOneriSistemi.Models
{
    public class SupportThread
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Subject { get; set; }

        public bool IsClosed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

        public int UnreadByAdminCount { get; set; } = 0;
        public int UnreadByUserCount { get; set; } = 0;

        public List<SupportMessage> Messages { get; set; } = new();
    }
}
