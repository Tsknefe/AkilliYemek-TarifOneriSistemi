using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Helpers;
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

        public async Task<NutritionFacts> GetNutritionAsync(string name, string englishName = null)
        {
            string key = englishName ?? name;

            // CACHE KONTROL
            if (NutritionCache.TryGet(key, out var cached))
                return cached;

            string searchTerm =
                englishName?.ToLower().Trim()
                ?? TranslationHelper.Translate(name)
                ?? name.ToLower().Trim();

            string url =
                $"https://world.openfoodfacts.org/cgi/search.pl?search_terms={searchTerm}&search_simple=1&json=true&page_size=5";

            try
            {
                string json = await _http.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(json);

                var products = doc.RootElement.GetProperty("products");
                if (products.GetArrayLength() == 0)
                    return null;

                NutritionFacts best = null;

                foreach (var product in products.EnumerateArray())
                {
                    if (!product.TryGetProperty("nutriments", out var nutr))
                        continue;

                    var n = new NutritionFacts
                    {
                        Calories = nutr.TryGetProperty("energy-kcal_100g", out var x1) ? x1.GetDouble() : 0,
                        Protein = nutr.TryGetProperty("proteins_100g", out var x2) ? x2.GetDouble() : 0,
                        Fat = nutr.TryGetProperty("fat_100g", out var x3) ? x3.GetDouble() : 0,
                        Carbs = nutr.TryGetProperty("carbohydrates_100g", out var x4) ? x4.GetDouble() : 0,
                        Sugar = nutr.TryGetProperty("sugars_100g", out var x5) ? x5.GetDouble() : 0,
                        Fiber = nutr.TryGetProperty("fiber_100g", out var x6) ? x6.GetDouble() : 0,
                        Sodium = nutr.TryGetProperty("sodium_100g", out var x7) ? x7.GetDouble() : 0,
                    };

                    if (n.Calories > 0)
                    {
                        best = n;
                        break;
                    }
                }

                if (best != null)
                    NutritionCache.Set(key, best);

                return best;
            }
            catch
            {
                return null;
            }
        }

        public async Task<NutritionFacts> SaveNutritionForRecipeAsync(int recipeId)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null)
                return null;

            double cal = 0, p = 0, f = 0, carb = 0, sugar = 0, fiber = 0, sodium = 0;

            foreach (var item in recipe.RecipeIngredients)
            {
                var nf100 = await GetNutritionAsync(item.Ingredient.Name, item.Ingredient.EnglishName);

                if (nf100 == null)
                    continue;

                double factor = item.CalculatedGrams / 100.0;

                cal += nf100.Calories * factor;
                p += nf100.Protein * factor;
                f += nf100.Fat * factor;
                carb += nf100.Carbs * factor;
                sugar += nf100.Sugar * factor;
                fiber += nf100.Fiber * factor;
                sodium += nf100.Sodium * factor;
            }

            var result = new NutritionFacts
            {
                RecipeId = recipeId,
                Calories = cal,
                Protein = p,
                Fat = f,
                Carbs = carb,
                Sugar = sugar,
                Fiber = fiber,
                Sodium = sodium
            };

            var existing = await _context.NutritionFacts.FirstOrDefaultAsync(x => x.RecipeId == recipeId);
            if (existing == null)
                _context.NutritionFacts.Add(result);
            else
            {
                existing.Calories = cal;
                existing.Protein = p;
                existing.Fat = f;
                existing.Carbs = carb;
                existing.Sugar = sugar;
                existing.Fiber = fiber;
                existing.Sodium = sodium;
            }

            await _context.SaveChangesAsync();
            return result;
        }
    }
}
