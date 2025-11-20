using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
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
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    r.Name.Contains(search) ||
                    r.Description.Contains(search) ||
                    r.DietType.Contains(search));
            }

            return await query.ToListAsync();
        }

        public async Task<Recipe?> GetByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.NutritionFacts)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipe> CreateAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<Recipe?> UpdateAsync(Recipe recipe)
        {
            var existing = await _context.Recipes.FindAsync(recipe.Id);
            if (existing == null)
                return null;

            existing.Name = recipe.Name;
            existing.Description = recipe.Description;
            existing.CookingTime = recipe.CookingTime;
            existing.Servings = recipe.Servings;
            existing.DietType = recipe.DietType;

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
    }
}
