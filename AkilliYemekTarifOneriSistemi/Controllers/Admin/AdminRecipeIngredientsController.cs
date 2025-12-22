using AkilliYemekTarifOneriSistemi.Models;
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

        [HttpGet]
        public async Task<IActionResult> Index(int recipeId)
        {
            if (recipeId <= 0) return BadRequest("recipeId geçersiz.");

            var recipe = await _recipeService.GetByIdAsync(recipeId);
            if (recipe == null) return NotFound();

            ViewBag.Recipe=recipe;

            ViewBag.Ingredients = new SelectList(
                await _ingredientService.GetAllAsync(),
                "Id",
                "Name"
            );

            var items = await _recipeIngredientService.GetByRecipeIdAsync(recipeId);

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int recipeId, int ingredientId, double amount, string unit)
        {
            if (recipeId <= 0) return BadRequest("recipeId geçersiz.");
            if (ingredientId <= 0)
            {
                TempData["ErrorMessage"] = "Lütfen bir malzeme seçin.";
                return RedirectToAction(nameof(Index), new { recipeId });
            }
            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Miktar 0'dan büyük olmalý.";
                return RedirectToAction(nameof(Index), new { recipeId });
            }
            if (string.IsNullOrWhiteSpace(unit))
            {
                TempData["ErrorMessage"] = "Birim boþ olamaz.";
                return RedirectToAction(nameof(Index), new { recipeId });
            }

            await _recipeIngredientService.AddAsync(recipeId, ingredientId, amount, unit.Trim());

            TempData["SuccessMessage"] = "Malzeme eklendi.";
            return RedirectToAction(nameof(Index), new { recipeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id, int recipeId)
        {
            if (recipeId <= 0) return BadRequest("recipeId geçersiz.");

            await _recipeIngredientService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Malzeme silindi.";
            return RedirectToAction(nameof(Index), new { recipeId });
        }
    }
}
