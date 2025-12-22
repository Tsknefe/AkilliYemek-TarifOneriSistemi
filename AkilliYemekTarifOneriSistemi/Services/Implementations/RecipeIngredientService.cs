using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Helpers;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    public class RecipeIngredientService : IRecipeIngredientService
    {
        private readonly ApplicationDbContext _context;
        private readonly INutritionService _nutritionService;

        public RecipeIngredientService(
            ApplicationDbContext context,
            INutritionService nutritionService)
        {
            _context = context;
            _nutritionService = nutritionService;
        }

        
        public async Task<List<RecipeIngredient>> GetByRecipeIdAsync(int recipeId)
        {
            return await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .Where(ri => ri.RecipeId == recipeId)
                .ToListAsync();
        }

        
        public async Task<RecipeIngredient?> GetByIdAsync(int id)
        {
            return await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .FirstOrDefaultAsync(ri => ri.Id == id);
        }

        
        public async Task<RecipeIngredient> AddAsync(
            int recipeId,
            int ingredientId,
            double quantity,
            string unit)
        {
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null)
                throw new InvalidOperationException("Ingredient bulunamadý");

            double grams = UnitConverter.ToGram(quantity, unit, ingredient.Name);

            var entity = new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Quantity = quantity.ToString(),
                Unit = unit,
                CalculatedGrams = grams
            };

            _context.RecipeIngredients.Add(entity);
            await _context.SaveChangesAsync();

            
            await _nutritionService.SaveNutritionForRecipeAsync(recipeId);

            return entity;
        }

        
        public async Task<RecipeIngredient?> UpdateAsync(
            int id,
            int ingredientId,
            double quantity,
            string unit)
        {
            var entity = await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .FirstOrDefaultAsync(ri => ri.Id == id);

            if (entity == null)
                return null;

            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null)
                throw new InvalidOperationException("Ingredient bulunamadý");

            double grams = UnitConverter.ToGram(quantity, unit, ingredient.Name);

            entity.IngredientId = ingredientId;
            entity.Quantity = quantity.ToString();
            entity.Unit = unit;
            entity.CalculatedGrams = grams;

            await _context.SaveChangesAsync();

            await _nutritionService.SaveNutritionForRecipeAsync(entity.RecipeId);

            return entity;
        }

        
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.RecipeIngredients.FindAsync(id);
            if (entity == null)
                return false;

            int recipeId = entity.RecipeId;

            _context.RecipeIngredients.Remove(entity);
            await _context.SaveChangesAsync();

            await _nutritionService.SaveNutritionForRecipeAsync(recipeId);

            return true;
        }

        
        public async Task<double> CalculateCaloriesAsync(int recipeIngredientId)
        {
            var ri = await _context.RecipeIngredients
                .Include(x => x.Ingredient)
                .FirstOrDefaultAsync(x => x.Id == recipeIngredientId);

            if (ri == null || ri.Ingredient == null)
                return 0;

            
            if (string.IsNullOrWhiteSpace(ri.Ingredient.EnglishName))
                return 0;

            var nutrition = await _nutritionService.GetNutritionAsync(
                ri.Ingredient.Name,
                ri.Ingredient.EnglishName
            );

            if (nutrition == null || nutrition.Calories <= 0)
                return 0;

            
            double totalCalories =
                (nutrition.Calories / 100.0) * ri.CalculatedGrams;

            return Math.Round(totalCalories, 2);
        }

    }
}
