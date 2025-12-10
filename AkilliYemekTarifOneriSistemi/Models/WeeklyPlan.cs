using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AkilliYemekTarifOneriSistemi.Models
{
    public class WeeklyPlan
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string? Title { get; set; }

        [BindNever]  // 🔥 BUNU EKLEDİK
        public IdentityUser User { get; set; } = null!;

        [BindNever]  // 🔥 BUNU EKLEDİK
        public ICollection<WeeklyPlanItem> Items { get; set; } = new List<WeeklyPlanItem>();
    }
}
