using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using AkilliYemekTarifOneriSistemi.Services.DietRules;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHealthProfileService _healthProfileService;
        private readonly IAllergyService _allergyService;

        public RecommendationService(
            ApplicationDbContext context,
            IHealthProfileService healthProfileService,
            IAllergyService allergyService)
        {
            _context = context;
            _healthProfileService = healthProfileService;
            _allergyService = allergyService;
        }

        public Task<List<RecommendationResult>> RecommendAsync(
            List<int> ingredientIds,
            int? maxCookingTime,
            double? targetCalories,
            string? dietType,
            int top)
        {
            return RecommendAsync(ingredientIds, maxCookingTime, targetCalories, dietType, top, mealType: null);
        }

        public Task<List<RecommendationResult>> RecommendForUserAsync(
            string userId,
            List<int> ingredientIds,
            int? maxCookingTime,
            int top)
        {
            return RecommendForUserAsync(userId, ingredientIds, maxCookingTime, top, mealType: null);
        }

        
        
        
        public async Task<List<RecommendationResult>> RecommendAsync(
            List<int> ingredientIds,
            int? maxCookingTime,
            double? targetCalories,
            string? dietType,
            int top,
            string? mealType)
        {
            ingredientIds ??= new List<int>();
            if (top <= 0) top = 10;

            var diet = DietTypeNormalizer.Normalize(dietType);
            var dietRestricted = DietTypeNormalizer.IsRestricted(diet);
            var mt = NormalizeMealType(mealType);

            var recipes = await _context.Recipes
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .Include(r => r.NutritionFacts)
                .ToListAsync();

            var results = new List<RecommendationResult>();

            foreach (var recipe in recipes)
            {
                
                if (dietRestricted)
                {
                    var texts = BuildDietTexts(recipe);
                    if (!DietRuleEngine.MatchesDiet(diet, texts))
                        continue;
                }

                if (!string.IsNullOrWhiteSpace(mt))
                {
                    if (!HasMealTag(recipe.MealTags, mt))
                        continue;
                }

                var res = CalculateScore(
                    recipe,
                    ingredientIds,
                    maxCookingTime,
                    targetCalories,
                    macroTargets: null,
                    mealType: mt
                );

                if (res.Score > 0)
                    results.Add(res);
            }

            return results
                .OrderByDescending(r => r.Score)
                .Take(top)
                .ToList();
        }

        
        
        
        public async Task<List<RecommendationResult>> RecommendForUserAsync(
            string userId,
            List<int> ingredientIds,
            int? maxCookingTime,
            int top,
            string? mealType)
        {
            ingredientIds ??= new List<int>();
            if (top <= 0) top = 10;

            
            var profile = await _healthProfileService.GetOrCreateProfileAsync(userId);

            var targetCalories = await _healthProfileService.GetTargetCaloriesAsync(userId);
            var macroTargets = await _healthProfileService.GetMacroTargetsAsync(userId);

            var profileDietType = DietTypeNormalizer.Normalize(profile?.DietType);
            var dietRestricted = DietTypeNormalizer.IsRestricted(profileDietType);

            var mt = NormalizeMealType(mealType);

            var allergicIngredientIds =
                await _allergyService.GetUserAllergyIngredientIdsAsync(userId) ?? new List<int>();

            var recipes = await _context.Recipes
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .Include(r => r.NutritionFacts)
                .ToListAsync();

            var results = new List<RecommendationResult>();

            foreach (var recipe in recipes)
            {
                
                if (recipe.RecipeIngredients != null && recipe.RecipeIngredients.Any() && allergicIngredientIds.Any())
                {
                    bool hasAllergen = recipe.RecipeIngredients
                        .Any(ri => allergicIngredientIds.Contains(ri.IngredientId));

                    if (hasAllergen)
                        continue;
                }

                
                if (dietRestricted)
                {
                    var texts = BuildDietTexts(recipe);
                    if (!DietRuleEngine.MatchesDiet(profileDietType, texts))
                        continue;
                }

                
                if (!string.IsNullOrWhiteSpace(mt))
                {
                    if (!HasMealTag(recipe.MealTags, mt))
                        continue;
                }

                
                var res = CalculateScore(
                    recipe,
                    ingredientIds,
                    maxCookingTime,
                    targetCalories,
                    macroTargets: macroTargets,
                    mealType: mt
                );

                
                var seed = HashCode.Combine(userId, recipe.Id, DateTime.Today);
                var rng = new Random(seed);
                res.Score += rng.NextDouble() * 0.03;

                if (res.Score > 0)
                    results.Add(res);
            }

            return results
                .OrderByDescending(r => r.Score)
                .Take(top)
                .ToList();
        }

        
        
        
        private static List<string> BuildDietTexts(Recipe recipe)
        {
            var texts = new List<string>();

            
            if (recipe.RecipeIngredients != null)
            {
                foreach (var ri in recipe.RecipeIngredients)
                {
                    if (ri?.Ingredient == null) continue;

                    if (!string.IsNullOrWhiteSpace(ri.Ingredient.Name))
                        texts.Add(ri.Ingredient.Name);

                    if (!string.IsNullOrWhiteSpace(ri.Ingredient.EnglishName))
                        texts.Add(ri.Ingredient.EnglishName);
                }
            }

            
            if (!string.IsNullOrWhiteSpace(recipe.Title)) texts.Add(recipe.Title);
            if (!string.IsNullOrWhiteSpace(recipe.Name)) texts.Add(recipe.Name);
            if (!string.IsNullOrWhiteSpace(recipe.Description)) texts.Add(recipe.Description);

            return texts;
        }

        
        
        
        private RecommendationResult CalculateScore(
            Recipe recipe,
            List<int> ingredientIds,
            int? maxCookingTime,
            double? targetCalories,
            (double proteinGr, double fatGr, double carbGr)? macroTargets,
            string? mealType)
        {
            ingredientIds ??= new List<int>();
            ingredientIds = ingredientIds.Distinct().ToList();

            
            double ingredientMatch = 0;
            if (recipe.RecipeIngredients != null && recipe.RecipeIngredients.Any() && ingredientIds.Any())
            {
                var recipeIngIds = recipe.RecipeIngredients.Select(ri => ri.IngredientId).Distinct().ToList();
                var common = recipeIngIds.Intersect(ingredientIds).Count();
                var total = recipeIngIds.Count;
                ingredientMatch = total > 0 ? (double)common / total : 0;
            }

            
            double timeFit = 1;
            if (maxCookingTime.HasValue && maxCookingTime.Value > 0)
            {
                if (recipe.CookingTime <= maxCookingTime.Value) timeFit = 1;
                else
                {
                    var diff = recipe.CookingTime - maxCookingTime.Value;
                    timeFit = Math.Max(0, 1 - (double)diff / maxCookingTime.Value);
                }
            }

            
            double mealFit = 1;
            if (!string.IsNullOrWhiteSpace(mealType))
                mealFit = HasMealTag(recipe.MealTags, mealType) ? 1 : 0;

            
            double calorieFit = 0;
            if (targetCalories.HasValue && targetCalories.Value > 0 &&
                recipe.NutritionFacts != null && recipe.NutritionFacts.Calories > 0)
            {
                var diff = Math.Abs(recipe.NutritionFacts.Calories - targetCalories.Value);
                var ratio = diff / targetCalories.Value;
                calorieFit = 1 / (1 + ratio);
            }

            
            double macroFit = 0;
            if (macroTargets.HasValue &&
                recipe.NutritionFacts != null &&
                recipe.NutritionFacts.Protein > 0 &&
                recipe.NutritionFacts.Fat > 0 &&
                recipe.NutritionFacts.Carbs > 0)
            {
                var (pT, fT, cT) = macroTargets.Value;

                double pDiff = Math.Abs(recipe.NutritionFacts.Protein - pT) / Math.Max(1, pT);
                double fDiff = Math.Abs(recipe.NutritionFacts.Fat - fT) / Math.Max(1, fT);
                double cDiff = Math.Abs(recipe.NutritionFacts.Carbs - cT) / Math.Max(1, cT);

                double avgDiff = (pDiff + fDiff + cDiff) / 3.0;
                macroFit = 1 / (1 + avgDiff);
            }

            
            const double wIngredient = 0.30;
            const double wTime = 0.10;
            const double wCalorie = 0.20;
            const double wMacro = 0.20;
            const double wMeal = 0.25;

            double score =
                  ingredientMatch * wIngredient
                + timeFit * wTime
                + calorieFit * wCalorie
                + macroFit * wMacro
                + mealFit * wMeal;

            return new RecommendationResult
            {
                Recipe = recipe,
                Score = score,
                IngredientMatch = ingredientMatch,
                TimeFit = timeFit,
                CalorieFit = calorieFit,
                MacroFit = macroFit,
                MealFit = mealFit,

                
                DietFit = 1,
                ProfileDietFit = 1
            };
        }

        
        
        
        private static string NormalizeMealType(string? mealType)
        {
            if (string.IsNullOrWhiteSpace(mealType)) return "";
            return mealType.Trim().ToLowerInvariant();
        }

        private static bool HasMealTag(string? mealTags, string mealType)
        {
            var mt = NormalizeMealType(mealType);
            if (string.IsNullOrWhiteSpace(mt)) return true;

            var tags = (mealTags ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.ToLowerInvariant());

            return tags.Contains(mt);
        }
    }
}
