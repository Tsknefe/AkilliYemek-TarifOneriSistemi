using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RecipesController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 📋 Tarif Listesi + 🔎 Arama (Title üzerinden)
        // /Recipes veya /Recipes?q=makarna
        [HttpGet]
        public async Task<IActionResult> Index(string? q)
        {
            IQueryable<Recipe> query = _context.Recipes
                .Include(r => r.NutritionFacts);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();

                // Title üzerinden arama
                query = query.Where(r => r.Title.Contains(q));
            }

            var recipes = await query
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            return View(recipes);
        }

        // 🔍 Tarif Detay
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.NutritionFacts)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
                return NotFound();

            // ❤️ Favori kontrolü
            bool isFavorite = false;

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    isFavorite = await _context.FavoriteRecipes
                        .AnyAsync(f => f.UserId == user.Id && f.RecipeId == recipe.Id);
                }
            }

            ViewBag.IsFavorite = isFavorite;
            return View(recipe);
        }

        // ❤️ Favoriye ekle
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavorite(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            bool exists = await _context.FavoriteRecipes
                .AnyAsync(f => f.UserId == user.Id && f.RecipeId == id);

            if (!exists)
            {
                _context.FavoriteRecipes.Add(new FavoriteRecipe
                {
                    UserId = user.Id,
                    RecipeId = id,
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // ❌ Favoriden çıkar
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var fav = await _context.FavoriteRecipes
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.RecipeId == id);

            if (fav != null)
            {
                _context.FavoriteRecipes.Remove(fav);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // ⭐ Favoriler
        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var recipes = await _context.FavoriteRecipes
                .Where(f => f.UserId == user.Id)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r.NutritionFacts)
                .Select(f => f.Recipe)
                .ToListAsync();

            return View(recipes);
        }
    }
}
