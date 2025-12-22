using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<RecommendationResult>> RecommendAsync(
            List<int> ingredientIds,
            int? maxCookingTime,
            double? targetCalories,
            string? dietType,
            int top);

        Task<List<RecommendationResult>> RecommendAsync(
            List<int> ingredientIds,
            int? maxCookingTime,
            double? targetCalories,
            string? dietType,
            int top,
            string? mealType);

        Task<List<RecommendationResult>> RecommendForUserAsync(
            string userId,
            List<int> ingredientIds,
            int? maxCookingTime,
            int top);

        Task<List<RecommendationResult>> RecommendForUserAsync(
            string userId,
            List<int> ingredientIds,
            int? maxCookingTime,
            int top,
            string? mealType);
    }

    public class RecommendationResult
    {
        public Recipe Recipe { get; set; } = null!;
        public double Score { get; set; }
        public double IngredientMatch { get; set; }
        public double TimeFit { get; set; }
        public double CalorieFit { get; set; }
        public double DietFit { get; set; }
        public double MacroFit { get; set; }
        public double ProfileDietFit { get; set; }

        
        public double MealFit { get; set; }
    }
}
