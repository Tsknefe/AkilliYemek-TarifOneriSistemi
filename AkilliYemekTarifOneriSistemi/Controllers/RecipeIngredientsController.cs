using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using AkilliYemekTarifOneriSistemi.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    public class RecipeIngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INutritionService _nutritionService;

        public RecipeIngredientsController(ApplicationDbContext context, INutritionService nutritionService)
        {
            _context = context;
            _nutritionService = nutritionService;
        }

        public async Task<IActionResult> Add(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return NotFound();

            ViewBag.RecipeId = id;
            ViewBag.Ingredients = await _context.Ingredients.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, int ingredientId, double quantity, string unit)
        {
            if (ingredientId == 0 || quantity <= 0 || string.IsNullOrWhiteSpace(unit))
            {
                ModelState.AddModelError("", "Lütfen tüm alanlarý doldurun");
                ViewBag.RecipeId = id;
                ViewBag.Ingredients = await _context.Ingredients.ToListAsync();
                return View();
            }

            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null)
                return NotFound();

            double grams = UnitConverter.ToGram(quantity, unit, ingredient.Name);

            var ri = new RecipeIngredient
            {
                RecipeId = id,
                IngredientId = ingredientId,
                Quantity = quantity.ToString(),
                Unit = unit,
                CalculatedGrams = grams
            };

            _context.RecipeIngredients.Add(ri);
            await _context.SaveChangesAsync();

            await _nutritionService.SaveNutritionForRecipeAsync(id);

            return RedirectToAction("Details", "Recipes", new { id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ri = await _context.RecipeIngredients
                .Include(x => x.Ingredient)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ri == null)
                return NotFound();

            ViewBag.Ingredients = await _context.Ingredients.ToListAsync();

            return View(ri);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, double quantity, string unit, int ingredientId)
        {
            var ri = await _context.RecipeIngredients
                .Include(r => r.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ri == null)
                return NotFound();

            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null)
                return NotFound();

            double grams = UnitConverter.ToGram(quantity, unit, ingredient.Name);

            ri.IngredientId = ingredientId;
            ri.Quantity = quantity.ToString();
            ri.Unit = unit;
            ri.CalculatedGrams = grams;

            await _context.SaveChangesAsync();

            await _nutritionService.SaveNutritionForRecipeAsync(ri.RecipeId);

            return RedirectToAction("Details", "Recipes", new { id = ri.RecipeId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ri = await _context.RecipeIngredients
                .Include(x => x.Ingredient)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ri == null)
                return NotFound();

            return View(ri);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ri = await _context.RecipeIngredients.FindAsync(id);
            if (ri == null)
                return NotFound();

            int recipeId = ri.RecipeId;

            _context.RecipeIngredients.Remove(ri);
            await _context.SaveChangesAsync();

            await _nutritionService.SaveNutritionForRecipeAsync(recipeId);

            return RedirectToAction("Details", "Recipes", new { id = recipeId });
        }
    }
}
