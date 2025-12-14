using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AkilliYemekTarifOneriSistemi.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminRecipeIngredientsController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IIngredientService _ingredientService;
        private readonly IRecipeIngredientService _recipeIngredientService;

        public AdminRecipeIngredientsController(
            IRecipeService recipeService,
            IIngredientService ingredientService,
            IRecipeIngredientService recipeIngredientService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
            _recipeIngredientService = recipeIngredientService;
        }

        // 📋 Tarifin malzemeleri
        public async Task<IActionResult> Index(int recipeId)
        {
            var recipe = await _recipeService.GetByIdAsync(recipeId);
            if (recipe == null)
                return NotFound();

            ViewBag.Recipe = recipe;

            ViewBag.Ingredients = new SelectList(
                await _ingredientService.GetAllAsync(),
                "Id",
                "Name"
            );

            var items = await _recipeIngredientService.GetByRecipeIdAsync(recipeId);

            return View(items);
        }

        // ➕ Malzeme ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            int recipeId,
            int ingredientId,
            double amount,
            string unit)
        {
            await _recipeIngredientService.AddAsync(
                recipeId,
                ingredientId,
                amount,
                unit
            );

            return RedirectToAction(nameof(Index), new { recipeId });
        }

        // ❌ Malzeme sil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id, int recipeId)
        {
            await _recipeIngredientService.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { recipeId });
        }
    }
}
