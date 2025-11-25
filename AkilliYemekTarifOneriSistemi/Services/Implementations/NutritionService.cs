using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    public class NutritionService : INutritionService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _http;

        public NutritionService(ApplicationDbContext context)
        {
            _context = context;
            _http = new HttpClient();
        }

        // ----------- 1) API'den malzeme besin değerlerini çek ------------
        public async Task<NutritionFacts> GetNutritionAsync(string ingredientName)
        {
            try
            {
                string url = $"https://world.openfoodfacts.org/cgi/search.pl?search_terms={ingredientName}&json=true";

                var response = await _http.GetStringAsync(url);

                using JsonDocument doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                var products = root.GetProperty("products");

                if (products.GetArrayLength() == 0)
                    return null;

                var nutriments = products[0].GetProperty("nutriments");

                NutritionFacts nf = new NutritionFacts
                {
                    Calories = nutriments.TryGetProperty("energy-kcal_100g", out var val1) ? val1.GetDouble() : 0,
                    Protein = nutriments.TryGetProperty("proteins_100g", out var val2) ? val2.GetDouble() : 0,
                    Fat = nutriments.TryGetProperty("fat_100g", out var val3) ? val3.GetDouble() : 0,
                    Carbs = nutriments.TryGetProperty("carbohydrates_100g", out var val4) ? val4.GetDouble() : 0,
                    Sugar = nutriments.TryGetProperty("sugars_100g", out var val5) ? val5.GetDouble() : 0,
                    Fiber = nutriments.TryGetProperty("fiber_100g", out var val6) ? val6.GetDouble() : 0,
                    Sodium = nutriments.TryGetProperty("sodium_100g", out var val7) ? val7.GetDouble() : 0
                };

                return nf;
            }
            catch
            {
                return null;
            }
        }

        // ----------- 2) Tarifin tüm malzemelerinden toplam besin değeri hesapla ------------
        public async Task<NutritionFacts> SaveNutritionForRecipeAsync(int recipeId)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null)
                return null;

            double totalCal = 0, totalProtein = 0, totalFat = 0, totalCarbs = 0, totalSugar = 0, totalFiber = 0, totalSodium = 0;

            foreach (var item in recipe.RecipeIngredients)
            {
                // 1) Malzemenin 100g değerini API'den al
                var nf100 = await GetNutritionAsync(item.Ingredient.Name);
                if (nf100 == null) continue;

                // 2) Kullanılan miktara göre ölçekle
                double factor = item.Quantity / 100.0;

                totalCal += nf100.Calories * factor;
                totalProtein += nf100.Protein * factor;
                totalFat += nf100.Fat * factor;
                totalCarbs += nf100.Carbs * factor;
                totalSugar += nf100.Sugar * factor;
                totalFiber += nf100.Fiber * factor;
                totalSodium += nf100.Sodium * factor;
            }

            var result = new NutritionFacts
            {
                RecipeId = recipeId,
                Calories = totalCal,
                Protein = totalProtein,
                Fat = totalFat,
                Carbs = totalCarbs,
                Sugar = totalSugar,
                Fiber = totalFiber,
                Sodium = totalSodium
            };

            // Eski değer varsa güncelle
            var existing = await _context.NutritionFacts
                .FirstOrDefaultAsync(x => x.RecipeId == recipeId);

            if (existing == null)
            {
                _context.NutritionFacts.Add(result);
            }
            else
            {
                existing.Calories = totalCal;
                existing.Protein = totalProtein;
                existing.Fat = totalFat;
                existing.Carbs = totalCarbs;
                existing.Sugar = totalSugar;
                existing.Fiber = totalFiber;
                existing.Sodium = totalSodium;
            }

            await _context.SaveChangesAsync();
            return result;
        }
    }
}
