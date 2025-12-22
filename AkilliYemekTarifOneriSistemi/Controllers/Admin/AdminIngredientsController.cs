using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminIngredientsController : Controller
    {
        private readonly IIngredientService _ingredientService;

        public AdminIngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        
        public async Task<IActionResult> Index(string? search)
        {
            var ingredients = await _ingredientService.GetAllAsync(search);
            return View(ingredients);
        }

        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,EnglishName")] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
                return View(ingredient);

            await _ingredientService.CreateAsync(ingredient);
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);
            if (ingredient == null)
                return NotFound();

            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,Name,EnglishName")] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
                return View(ingredient);

            await _ingredientService.UpdateAsync(ingredient);
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);
            if (ingredient == null)
                return NotFound();

            return View(ingredient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ingredientService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
