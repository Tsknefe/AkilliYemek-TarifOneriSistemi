using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminRecipesController : Controller
    {
        private readonly IRecipeService _recipeService;

        public AdminRecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            var recipes = await _recipeService.GetAllAsync(search);
            return View(recipes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Recipe());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recipe recipe, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(recipe);

            recipe.MealTags = NormalizeMealTags(recipe.MealTags);
            recipe.DietType = NormalizeDietType(recipe.DietType);

            if (imageFile != null && imageFile.Length > 0)
                recipe.ImageUrl = await SaveRecipeImageAsync(imageFile);

            await _recipeService.CreateAsync(recipe);

            TempData["SuccessMessage"] = "Tarif baþarýyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
                return NotFound();

            
            recipe.MealTags = NormalizeMealTags(recipe.MealTags);
            recipe.DietType = NormalizeDietType(recipe.DietType);

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Recipe recipe, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(recipe);

            var existing = await _recipeService.GetByIdAsync(recipe.Id);
            if (existing == null)
                return NotFound();

            recipe.MealTags = NormalizeMealTags(recipe.MealTags);
            recipe.DietType = NormalizeDietType(recipe.DietType);

            if (imageFile != null && imageFile.Length > 0)
                recipe.ImageUrl = await SaveRecipeImageAsync(imageFile);
            else
                recipe.ImageUrl = existing.ImageUrl;

            var updated = await _recipeService.UpdateAsync(recipe);
            if (updated == null)
                return NotFound();

            TempData["SuccessMessage"] = "Tarif güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _recipeService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Tarif silindi.";
            return RedirectToAction(nameof(Index));
        }

        private static async Task<string> SaveRecipeImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "uploads", "recipes");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return "/uploads/recipes/" + fileName;
        }

        private static string NormalizeMealTags(string? tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
                return "";

            var allowed = new HashSet<string> { "breakfast", "lunch", "dinner", "snack" };

            var cleaned = tags
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.Trim().ToLower())
                .Where(x => allowed.Contains(x))
                .Distinct();

            return string.Join(",", cleaned);
        }

        private static string NormalizeDietType(string? diet)
        {
            if (string.IsNullOrWhiteSpace(diet))
                return "";

            var d = diet.Trim().ToLower();
            if (d == "vejeteryan") d = "vejetaryen";
            return d;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutoAssignMealTags()
        {
            var recipes = await _recipeService.GetAllWithCategoryAsync();

            int updatedCount = 0;

            foreach (var r in recipes)
            {
                if (string.IsNullOrWhiteSpace(r.MealTags))
                {
                    await _recipeService.UpdateAsync(r);
                    updatedCount++;
                }
            }

            TempData["SuccessMessage"] = $"MealTags otomatik atandý. Güncellenen tarif: {updatedCount}";
            return RedirectToAction(nameof(Index));
        }
    }
}
