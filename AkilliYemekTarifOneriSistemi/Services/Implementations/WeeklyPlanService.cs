using AkilliYemekTarifOneriSistemi.Services.Interfaces;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    // bu servis haftalık beslenme planını oluşturan kısım
    // yani kullanıcının profilini baz alıp 7 gün 3 öğün olacak şekilde plan çıkarıyor
    // arka tarafta öneri motorunu kullanıyor çünkü hangi tarifin uygun olduğuna karar verip çekmek gerekiyor
    public class WeeklyPlanService : IWeeklyPlanService
    {
        private readonly IHealthProfileService _healthProfileService;
        private readonly IRecommendationService _recommendationService;

        public WeeklyPlanService(
            IHealthProfileService healthProfileService,
            IRecommendationService recommendationService)
        {
            _healthProfileService = healthProfileService;
            _recommendationService = recommendationService;
        }

        public async Task<WeeklyPlanDto?> GenerateWeeklyPlanAsync(string userId, DateTime? startDate = null)
        {
            // burada ilk olarak kullanıcının günlük hedef kalorisine bakıyoruz
            // çünkü planın temeli buna göre kuruluyor
            var targetCalories = await _healthProfileService.GetTargetCaloriesAsync(userId);

            // kullanıcı profilini doldurmamışsa değer dönmez
            // bu durumda plan oluşturamıyoruz
            if (!targetCalories.HasValue)
                return null;

            double dailyTarget = targetCalories.Value;

            // şimdi kullanıcının profiline göre en uygun tarifleri çekiyoruz
            // alerji diyeti hedef kalorisi makroları hepsi recommendation service içinde zaten değerlendiriliyor
            // 7 gün 3 öğün için toplam 21 tarif yeterli oluyor
            var recommended = await _recommendationService.RecommendForUserAsync(
                userId,
                ingredientIds: new List<int>(), // burada evdeki malzeme kısıtlaması yok tamamen profil üzerinden gidiyoruz
                maxCookingTime: null,
                top: 21);

            // hiçbir tarif dönmediyse planı kuramayız
            if (recommended.Count == 0)
                return null;

            // burada planın başlangıç tarihini belirliyoruz
            // dışarıdan tarih verilmişse onu kullanıyoruz
            // verilmemişse bugünü alıyoruz
            var start = (startDate ?? DateTime.Today).Date;

            // haftalık plan objesini oluşturuyoruz
            // bitiş tarihi 7 günün sonu olacak şekilde hesaplanıyor
            var weeklyPlan = new WeeklyPlanDto
            {
                StartDate = start,
                EndDate = start.AddDays(6),
                TargetDailyCalories = dailyTarget
            };

            // sabit öğün tiplerimiz
            string[] mealTypes = new[] { "Kahvaltı", "Öğle", "Akşam" };

            // recommended listesinden sırayla tarif seçmek için kullanılan index
            int recipeIndex = 0;

            // şimdi 7 günlük planı oluşturuyoruz
            for (int dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                // her gün için tek bir day bilgisi oluşturuyoruz
                var day = new DailyPlanDto
                {
                    Date = start.AddDays(dayOffset)
                };

                // her gün 3 öğün oluşturuyoruz
                for (int mealIndex = 0; mealIndex < mealTypes.Length; mealIndex++)
                {
                    // tarifler biterse döngüyü kesiyoruz
                    if (recipeIndex >= recommended.Count)
                        break;

                    var rec = recommended[recipeIndex].Recipe;
                    recipeIndex++;

                    double cal = rec.NutritionFacts?.Calories ?? 0;

                    // günlük öğün listesine item ekliyoruz
                    day.Meals.Add(new MealItemDto
                    {
                        MealType = mealTypes[mealIndex],
                        RecipeId = rec.Id,
                        RecipeName = rec.Name,
                        Description = rec.Description,
                        Calories = cal,
                        CookingTime = rec.CookingTime,
                        DietType = rec.DietType
                    });
                }

                // oluşturduğumuz gün weeklyPlan içine ekleniyor
                weeklyPlan.Days.Add(day);
            }

            // finalde haftalık planı dönüyoruz
            return weeklyPlan;
        }
    }
}
