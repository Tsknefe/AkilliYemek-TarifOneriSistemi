using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IIngredientService _ingredientService;

        public AdminController(
            IRecipeService recipeService,
            IIngredientService ingredientService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
        }

        public async Task<IActionResult> Index()
        {
            var recipes = await _recipeService.GetAllAsync();
            var ingredients = await _ingredientService.GetAllAsync();

            ViewBag.RecipeCount = recipes.Count;
            ViewBag.IngredientCount = ingredients.Count;
            ViewBag.LatestRecipes = recipes
                .OrderByDescending(r => r.Id)
                .Take(5)
                .ToList();

            return View();
        }
    }
}
