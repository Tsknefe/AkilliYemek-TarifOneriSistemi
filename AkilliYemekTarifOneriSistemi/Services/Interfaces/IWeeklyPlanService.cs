using System;
using System.Threading.Tasks;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IWeeklyPlanService
    {
        /// <summary>
        /// Kullanıcının profil, alerji ve öneri motoruna göre 7 günlük plan oluşturur.
        /// </summary>
        Task<WeeklyPlanDto?> GenerateWeeklyPlanAsync(string userId, DateTime? startDate = null);
    }

    // Basit DTO’lar – istersen Models/Dto klasörüne ayırabilirsin
    public class WeeklyPlanDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TargetDailyCalories { get; set; }

        public List<DailyPlanDto> Days { get; set; } = new();
    }

    public class DailyPlanDto
    {
        public DateTime Date { get; set; }
        public List<MealItemDto> Meals { get; set; } = new();

        public double TotalCalories => Meals.Sum(m => m.Calories);
    }

    public class MealItemDto
    {
        public string MealType { get; set; } = string.Empty;
        public int RecipeId { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public double Calories { get; set; }
        public int CookingTime { get; set; }
        public string DietType { get; set; } = string.Empty;
    }
}
