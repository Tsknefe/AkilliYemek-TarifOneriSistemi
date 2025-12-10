using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AkilliYemekTarifOneriSistemi.Models
{
    public class WeeklyPlanItem
    {
        public int Id { get; set; }
        public int WeeklyPlanId { get; set; }
        public int RecipeId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string MealType { get; set; } = string.Empty;

        [BindNever]  // 🔥 Navigation bind edilmesin
        public WeeklyPlan WeeklyPlan { get; set; } = null!;

        [BindNever]  // 🔥 Navigation bind edilmesin
        public Recipe Recipe { get; set; } = null!;
    }
}
