using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    // bu servis kullanıcının malzemelerine diyetine kalorisine hedeflerine göre
    // en uygun tarifleri hesaplayan kısım
    // hem eski düz manuel filtreleme var hem de kullanıcı profiline göre çalışan gelişmiş sürüm var
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

        // bu eski yöntem dışarıdan parametre verilip api üzerinden direkt kullanılabiliyor
        // yani kullanıcı profiline göre değil tamamen requestte gelen verilere göre öneri yapıyor
        public async Task<List<RecommendationResult>> RecommendAsync(
            List<int> ingredientIds,
            int? maxCookingTime,
            double? targetCalories,
            string? dietType,
            int top)
        {
            // tüm tarifleri malzemeleri ve besin değerleri ile birlikte çek
            var query = _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.NutritionFacts)
                .AsQueryable();

            // diyet tipi seçilmişse sadece ona uyanları getir
            if (!string.IsNullOrWhiteSpace(dietType))
            {
                var d = dietType.Trim().ToLower();
                query = query.Where(r => r.DietType.ToLower() == d);
            }

            var recipes = await query.ToListAsync();
            var results = new List<RecommendationResult>();

            // her tarif için skor hesapla
            foreach (var recipe in recipes)
            {
                var res = CalculateScore(
                    recipe,
                    ingredientIds,
                    maxCookingTime,
                    targetCalories,
                    dietType,
                    macroTargets: null,
                    profileDietType: null);

                if (res.Score > 0)
                    results.Add(res);
            }

            // en yüksek skor alanları getir
            return results
                .OrderByDescending(r => r.Score)
                .Take(top)
                .ToList();
        }

        // bu yeni yöntem kullanıcı bilgilerini tamamen hesaba katarak çalışıyor
        // yani yaş boy kilo hedef aktivite seviyesi alerji diyet tipi hepsi dahil
        public async Task<List<RecommendationResult>> RecommendForUserAsync(
            string userId,
            List<int> ingredientIds,
            int? maxCookingTime,
            int top)
        {
            // kullanıcı profilini çekiyoruz
            var profile = await _healthProfileService.GetProfileAsync(userId);

            // günlük ortalama hedef kalori
            var targetCalories = await _healthProfileService.GetTargetCaloriesAsync(userId);

            // protein yağ karbonhidrat hedefleri
            var macroTargets = await _healthProfileService.GetMacroTargetsAsync(userId);

            // kullanıcının profilindeki diyet türü mesela vegan glutenfree falan
            string? profileDietType = profile?.DietType;

            // alerjileri alıyoruz
            var allergicIngredientIds = await _allergyService.GetUserAllergyIngredientIdsAsync(userId);

            // tarifleri getir
            var query = _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.NutritionFacts)
                .AsQueryable();

            // kullanıcı vegan seçtiyse sadece vegan tarifler gelsin
            if (!string.IsNullOrWhiteSpace(profileDietType))
            {
                var dt = profileDietType.Trim().ToLower();
                query = query.Where(r => r.DietType.ToLower() == dt);
            }

            var recipes = await query.ToListAsync();
            var results = new List<RecommendationResult>();

            foreach (var recipe in recipes)
            {
                // tarif içinde kullanıcının alerjik olduğu malzeme varsa direkt ele
                if (recipe.RecipeIngredients != null && allergicIngredientIds.Any())
                {
                    bool hasAllergen = recipe.RecipeIngredients
                        .Any(ri => allergicIngredientIds.Contains(ri.IngredientId));

                    if (hasAllergen)
                        continue;
                }

                // skor hesaplama
                var res = CalculateScore(
                    recipe,
                    ingredientIds,
                    maxCookingTime,
                    targetCalories,
                    dietType: profileDietType,
                    macroTargets: macroTargets,
                    profileDietType: profileDietType);

                if (res.Score > 0)
                    results.Add(res);
            }

            return results
                .OrderByDescending(r => r.Score)
                .Take(top)
                .ToList();
        }

        // bu fonksiyon bir tarifin ne kadar uygun olduğunu hesaplıyor
        // bütün puanlamanın mantığı burada dönüyor
        private RecommendationResult CalculateScore(
            Recipe recipe,
            List<int> ingredientIds,
            int? maxCookingTime,
            double? targetCalories,
            string? dietType,
            (double proteinGr, double fatGr, double carbGr)? macroTargets,
            string? profileDietType)
        {
            ingredientIds = ingredientIds?.Distinct().ToList() ?? new List<int>();

            // kullanıcının elindeki malzemeler tarifle ne kadar örtüşüyor
            double ingredientMatch = 0;
            if (recipe.RecipeIngredients != null && recipe.RecipeIngredients.Any() && ingredientIds.Any())
            {
                var recipeIngIds = recipe.RecipeIngredients.Select(ri => ri.IngredientId).Distinct();
                var common = recipeIngIds.Intersect(ingredientIds).Count();
                var total = recipeIngIds.Count();
                ingredientMatch = total > 0 ? (double)common / total : 0;
            }

            // tarifin süresi max süreye göre ne kadar uyuyor
            double timeFit = 1;
            if (maxCookingTime.HasValue && maxCookingTime.Value > 0)
            {
                if (recipe.CookingTime <= maxCookingTime.Value)
                {
                    timeFit = 1;
                }
                else
                {
                    var diff = recipe.CookingTime - maxCookingTime.Value;
                    timeFit = Math.Max(0, 1 - (double)diff / maxCookingTime.Value);
                }
            }

            // tarifin kalorisi hedef kaloriye ne kadar yakın
            double calorieFit = 0;
            if (targetCalories.HasValue && targetCalories.Value > 0 &&
                recipe.NutritionFacts != null && recipe.NutritionFacts.Calories > 0)
            {
                var diff = Math.Abs(recipe.NutritionFacts.Calories - targetCalories.Value);
                var ratio = diff / targetCalories.Value;
                calorieFit = 1 / (1 + ratio);
            }

            // kişinin seçtiği diyet tipi ile tarif uyumlu mu
            double dietFit = 1;
            if (!string.IsNullOrWhiteSpace(dietType))
            {
                var dt = dietType.Trim().ToLower();
                dietFit = recipe.DietType?.Trim().ToLower() == dt ? 1 : 0;
            }

            // profilindeki diyet türüne ekstra bonus
            double profileDietFit = 1;
            if (!string.IsNullOrWhiteSpace(profileDietType))
            {
                var pdt = profileDietType.Trim().ToLower();
                profileDietFit = recipe.DietType?.Trim().ToLower() == pdt ? 1 : 0.5;
            }

            // makro hedeflerine göre uyumluluk
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

            // ağırlıklar
            const double wIngredient = 0.30;
            const double wTime = 0.10;
            const double wCalorie = 0.20;
            const double wDiet = 0.10;
            const double wProfileDiet = 0.10;
            const double wMacro = 0.20;

            double score =
                  ingredientMatch * wIngredient
                + timeFit * wTime
                + calorieFit * wCalorie
                + dietFit * wDiet
                + profileDietFit * wProfileDiet
                + macroFit * wMacro;

            return new RecommendationResult
            {
                Recipe = recipe,
                Score = score,
                IngredientMatch = ingredientMatch,
                TimeFit = timeFit,
                CalorieFit = calorieFit,
                DietFit = dietFit,
                MacroFit = macroFit,
                ProfileDietFit = profileDietFit
            };
        }
    }
}
