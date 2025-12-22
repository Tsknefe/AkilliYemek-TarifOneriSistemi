using System;

namespace AkilliYemekTarifOneriSistemi.Models
{
    
    
    
    
    public class WeeklyPlanItem
    {
        
        
        
        public int Id { get; set; }

        
        
        
        public int WeeklyPlanId { get; set; }

        
        
        
        public int RecipeId { get; set; }

        
        
        
        
        public DayOfWeek DayOfWeek { get; set; }

        
        
        
        
        
        public string MealType { get; set; } = string.Empty;

        
        
        
        public WeeklyPlan WeeklyPlan { get; set; } = null!;

        
        
        
        public Recipe Recipe { get; set; } = null!;
    }
}



