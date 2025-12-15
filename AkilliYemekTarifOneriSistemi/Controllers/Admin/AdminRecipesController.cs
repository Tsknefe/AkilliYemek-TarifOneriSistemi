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

        // ===============================
        // LIST
        // ===============================
        public async Task<IActionResult> Index(string? search)
        {
            var recipes = await _recipeService.GetAllAsync(search);
            return View(recipes);
        }

        // ===============================
        // CREATE (GET)
        // ===============================
        public IActionResult Create()
        {
            return View();
        }

        // ===============================
        // CREATE (POST) ✅ TEK OLMALI
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Recipe recipe,
            IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(recipe);

            // 📸 Fotoğraf yüklendiyse
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/recipes");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                recipe.ImageUrl = "/uploads/recipes/" + fileName;
            }

            await _recipeService.CreateAsync(recipe);

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // EDIT (GET)
        // ===============================
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        // ===============================
        // EDIT (POST)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Recipe recipe)
        {
            if (!ModelState.IsValid)
                return View(recipe);

            var updated = await _recipeService.UpdateAsync(recipe);
            if (updated == null)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // DELETE (GET)
        // ===============================
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        // ===============================
        // DELETE (POST)
        // ===============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _recipeService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
