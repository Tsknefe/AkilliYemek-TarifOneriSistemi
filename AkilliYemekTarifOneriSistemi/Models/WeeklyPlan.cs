using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AkilliYemekTarifOneriSistemi.Models
{
    
    
    
    
    public class WeeklyPlan
    {
        
        
        
        public int Id { get; set; }

        
        
        
        public string UserId { get; set; } = string.Empty;

        
        
        
        
        public DateTime StartDate { get; set; }

        
        
        
        
        public string? Title { get; set; }

        
        
        
        public IdentityUser User { get; set; } = null!;
        public string DietTypeSnapshot { get; set; } = "normal";
        public double TargetCaloriesSnapshot { get; set; }


        
        
        
        
        public ICollection<WeeklyPlanItem> Items { get; set; } = new List<WeeklyPlanItem>();
    }
}



