using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.DietRules;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext _context;

        public RecipeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetAllAsync(string? search = null)
        {
            var query = _context.Recipes
                .Include(r => r.NutritionFacts)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();

                query = query.Where(r =>
                    (r.Title != null && r.Title.Contains(s)) ||
                    (r.Name != null && r.Name.Contains(s)) ||
                    (r.Description != null && r.Description.Contains(s)) ||
                    (r.DietType != null && r.DietType.Contains(s)));
            }

            return await query.ToListAsync();
        }

        
        public async Task<List<Recipe>> GetAllWithCategoryAsync(string? search = null)
        {
            var query = _context.Recipes
                .Include(r => r.Category) 
                .Include(r => r.NutritionFacts)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();

                query = query.Where(r =>
                    (r.Title != null && r.Title.Contains(s)) ||
                    (r.Name != null && r.Name.Contains(s)) ||
                    (r.Description != null && r.Description.Contains(s)) ||
                    (r.DietType != null && r.DietType.Contains(s)));
            }

            return await query.ToListAsync();
        }

        public async Task<Recipe?> GetByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.Category) 
                .Include(r => r.NutritionFacts)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipe> CreateAsync(Recipe recipe)
        {
            
            if (recipe.CategoryId.HasValue)
            {
                recipe.Category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == recipe.CategoryId.Value);
            }
            recipe.DietType = DietTypeNormalizer.Normalize(recipe.DietType);


            
            recipe.MealTags = PrepareMealTags(recipe.MealTags, recipe);

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<Recipe?> UpdateAsync(Recipe recipe)
        {
            var existing = await _context.Recipes
                .Include(r => r.Category) 
                .FirstOrDefaultAsync(r => r.Id == recipe.Id);

            existing.DietType = DietTypeNormalizer.Normalize(recipe.DietType);

            if (existing == null)
                return null;

            existing.Name = recipe.Name;
            existing.Description = recipe.Description;
            existing.CookingTime = recipe.CookingTime;
            existing.Servings = recipe.Servings;
            existing.DietType = recipe.DietType;
            existing.Instructions = recipe.Instructions;
            existing.ImagePath = recipe.ImagePath;
            existing.Title = recipe.Title;
            existing.ShortDescription = recipe.ShortDescription;
            existing.ImageUrl = recipe.ImageUrl;
            existing.PreparationTimeMinutes = recipe.PreparationTimeMinutes;
            existing.Difficulty = recipe.Difficulty;
            existing.CategoryId = recipe.CategoryId;

            
            if (recipe.CategoryId.HasValue)
            {
                existing.Category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == recipe.CategoryId.Value);
            }
            else
            {
                existing.Category = null;
            }

            
            existing.MealTags = PrepareMealTags(recipe.MealTags, existing);

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return false;

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return true;
        }

        

        private static string PrepareMealTags(string? incomingMealTags, Recipe recipeForCategory)
        {
            
            if (!string.IsNullOrWhiteSpace(incomingMealTags))
                return NormalizeMealTags(incomingMealTags);

            
            return AutoAssignMealTagsByCategory(recipeForCategory);
        }

        private static string AutoAssignMealTagsByCategory(Recipe recipe)
        {
            if (recipe.Category == null || string.IsNullOrWhiteSpace(recipe.Category.Name))
                return "";

            var category = recipe.Category.Name.Trim().ToLower();

            return category switch
            {
                "kahvaltý" => "breakfast",

                
                "çorba" => "lunch",
                "salata" => "lunch",

                
                "ana yemek" => "dinner",
                "makarna" => "dinner",

                
                "vejetaryen" => "lunch,dinner",

                
                "tatlý" => "snack",
                "atýþtýrmalýk" => "snack",

                _ => ""
            };
        }
    
        private static string NormalizeMealTags(string tags)
        {
            var allowed = new HashSet<string> { "breakfast", "lunch", "dinner", "snack" };

            var cleaned = tags
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.Trim().ToLower())
                .Where(x => allowed.Contains(x))
                .Distinct();

            return string.Join(",", cleaned);
        }
    }
}
