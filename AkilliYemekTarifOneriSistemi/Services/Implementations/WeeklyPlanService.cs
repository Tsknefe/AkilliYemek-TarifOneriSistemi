using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using AkilliYemekTarifOneriSistemi.Services.DietRules;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    public class WeeklyPlanService : IWeeklyPlanService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHealthProfileService _healthProfileService;
        private readonly IRecommendationService _recommendationService;

        public WeeklyPlanService(
            ApplicationDbContext context,
            IHealthProfileService healthProfileService,
            IRecommendationService recommendationService)
        {
            _context = context;
            _healthProfileService = healthProfileService;
            _recommendationService = recommendationService;
        }

        
        public async Task<WeeklyPlanDto?> GenerateWeeklyPlanAsync(string userId, DateTime? startDate = null)
        {
            var profile = await _healthProfileService.GetOrCreateProfileAsync(userId);
            Console.WriteLine($"[WeeklyPlan] profileExists={(profile != null)} userId={userId}");

            var targetCalories = await _healthProfileService.GetTargetCaloriesAsync(userId);
            Console.WriteLine($"[WeeklyPlan] targetCalories={(targetCalories.HasValue ? targetCalories.Value.ToString() : "NULL")}");

            if (!targetCalories.HasValue)
            {
                Console.WriteLine("[WeeklyPlan] targetCalories is NULL -> returning null");
                return null;
            }

            
            var currentDiet = DietTypeNormalizer.Normalize(profile?.DietType);

            
            var generalRecommended = await _recommendationService.RecommendForUserAsync(
                userId,
                ingredientIds: new List<int>(),
                maxCookingTime: null,
                top: 120);

            Console.WriteLine($"[WeeklyPlan] generalRecommendedCount={generalRecommended.Count}");

            if (generalRecommended.Count == 0)
            {
                Console.WriteLine("[WeeklyPlan] generalRecommended is 0 -> returning null");
                return null;
            }

            
            var breakfastRecommended = await _recommendationService.RecommendForUserAsync(
                userId,
                ingredientIds: new List<int>(),
                maxCookingTime: null,
                top: 60,
                mealType: "breakfast");

            var lunchRecommended = await _recommendationService.RecommendForUserAsync(
                userId,
                ingredientIds: new List<int>(),
                maxCookingTime: null,
                top: 60,
                mealType: "lunch");

            var dinnerRecommended = await _recommendationService.RecommendForUserAsync(
                userId,
                ingredientIds: new List<int>(),
                maxCookingTime: null,
                top: 60,
                mealType: "dinner");

            Console.WriteLine($"[WeeklyPlan] breakfast={breakfastRecommended.Count} lunch={lunchRecommended.Count} dinner={dinnerRecommended.Count}");

            
            if (breakfastRecommended.Count == 0) breakfastRecommended = generalRecommended;
            if (lunchRecommended.Count == 0) lunchRecommended = generalRecommended;
            if (dinnerRecommended.Count == 0) dinnerRecommended = generalRecommended;

            var start = (startDate ?? DateTime.Today).Date;

            var weeklyPlan = new WeeklyPlanDto
            {
                StartDate = start,
                EndDate = start.AddDays(6),
                TargetDailyCalories = targetCalories.Value,
            };

            int bIndex = 0, lIndex = 0, dIndex = 0;

            for (int dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                var day = new DailyPlanDto { Date = start.AddDays(dayOffset) };

                
                var usedToday = new HashSet<int>();

                var bRec = PickNextDistinct(breakfastRecommended, usedToday, ref bIndex);
                day.Meals.Add(MapToMealItemDto("Kahvaltý", bRec));

                var lRec = PickNextDistinct(lunchRecommended, usedToday, ref lIndex);
                day.Meals.Add(MapToMealItemDto("Öðle", lRec));

                var dRec = PickNextDistinct(dinnerRecommended, usedToday, ref dIndex);
                day.Meals.Add(MapToMealItemDto("Akþam", dRec));

                weeklyPlan.Days.Add(day);
            }

            return weeklyPlan;
        }

        private static Recipe PickNextDistinct(List<RecommendationResult> list, HashSet<int> usedIds, ref int index)
        {
            if (list == null || list.Count == 0)
                throw new InvalidOperationException("Öneri listesi boþ.");

            for (int i = 0; i < list.Count; i++)
            {
                var rec = list[index % list.Count].Recipe;
                index++;

                if (rec != null && usedIds.Add(rec.Id))
                    return rec;
            }

            
            return list[(index - 1 + list.Count) % list.Count].Recipe;
        }

        private static MealItemDto MapToMealItemDto(string mealType, Recipe rec)
        {
            return new MealItemDto
            {
                MealType = mealType,
                RecipeId = rec.Id,
                RecipeName = rec.Title,
                Description = rec.Description,
                Calories = rec.NutritionFacts?.Calories ?? 0,
                CookingTime = rec.CookingTime,
                DietType = rec.DietType ?? ""
            };
        }

        
        public async Task<int> SaveGeneratedPlanAsync(string userId, WeeklyPlanDto dto)
        {
            bool exists = await _context.WeeklyPlans
                .AnyAsync(x => x.UserId == userId && x.StartDate.Date == dto.StartDate.Date);

            if (exists)
                throw new InvalidOperationException("Bu hafta için zaten bir planýnýz var.");

            
            var profile = await _healthProfileService.GetOrCreateProfileAsync(userId);
            var currentDiet = DietTypeNormalizer.Normalize(profile?.DietType);
            var targetCalories = await _healthProfileService.GetTargetCaloriesAsync(userId) ?? 0;

            var plan = new WeeklyPlan
            {
                UserId = userId,
                Title = "Otomatik Haftalýk Plan",
                StartDate = dto.StartDate,

                
                DietTypeSnapshot = currentDiet,
                TargetCaloriesSnapshot = targetCalories
            };

            _context.WeeklyPlans.Add(plan);
            await _context.SaveChangesAsync();

            foreach (var day in dto.Days)
                foreach (var meal in day.Meals)
                {
                    _context.WeeklyPlanItems.Add(new WeeklyPlanItem
                    {
                        WeeklyPlanId = plan.Id,
                        DayOfWeek = day.Date.DayOfWeek,
                        MealType = meal.MealType,
                        RecipeId = meal.RecipeId
                    });
                }

            await _context.SaveChangesAsync();
            return plan.Id;
        }

        public async Task<int> SaveOrReplaceGeneratedPlanAsync(string userId, WeeklyPlanDto dto)
        {
            var existingPlan = await _context.WeeklyPlans
                .Include(wp => wp.Items)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.StartDate.Date == dto.StartDate.Date);

            if (existingPlan != null)
            {
                _context.WeeklyPlanItems.RemoveRange(existingPlan.Items);
                _context.WeeklyPlans.Remove(existingPlan);
                await _context.SaveChangesAsync();
            }

            
            var profile = await _healthProfileService.GetOrCreateProfileAsync(userId);
            var currentDiet = DietTypeNormalizer.Normalize(profile?.DietType);
            var targetCalories = await _healthProfileService.GetTargetCaloriesAsync(userId) ?? 0;

            var plan = new WeeklyPlan
            {
                UserId = userId,
                Title = "Otomatik Haftalýk Plan",
                StartDate = dto.StartDate,

                
                DietTypeSnapshot = currentDiet,
                TargetCaloriesSnapshot = targetCalories
            };

            _context.WeeklyPlans.Add(plan);
            await _context.SaveChangesAsync();

            foreach (var day in dto.Days)
                foreach (var meal in day.Meals)
                {
                    _context.WeeklyPlanItems.Add(new WeeklyPlanItem
                    {
                        WeeklyPlanId = plan.Id,
                        DayOfWeek = day.Date.DayOfWeek,
                        MealType = meal.MealType,
                        RecipeId = meal.RecipeId
                    });
                }

            await _context.SaveChangesAsync();
            return plan.Id;
        }
    }
}
