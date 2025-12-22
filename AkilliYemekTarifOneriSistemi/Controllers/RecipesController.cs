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
        private readonly ILogger<RecipesController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public RecipesController(
            ApplicationDbContext context,
            ILogger<RecipesController> logger,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var recipes = await _context.Recipes
                .Include(r => r.Category) 
                .Include(r => r.RecipeTags)
                    .ThenInclude(rt => rt.Tag) 
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            return View(recipes);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.RecipeTags)
                    .ThenInclude(rt => rt.Tag)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var isFavorite = await _context.FavoriteRecipes
                        .AnyAsync(fr => fr.UserId == currentUser.Id && fr.RecipeId == recipe.Id);
                    ViewBag.IsFavorite = isFavorite;
                }
            }
            else
            {
                ViewBag.IsFavorite = false;
            }

            return View(recipe);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavorite(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var existingFavorite = await _context.FavoriteRecipes
                .FirstOrDefaultAsync(fr => fr.UserId == currentUser.Id && fr.RecipeId == id);

            if (existingFavorite == null)
            {
                var favorite = new FavoriteRecipe
                {
                    UserId = currentUser.Id,
                    RecipeId = id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.FavoriteRecipes.Add(favorite);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tarif favorilerinize eklendi!";
            }
            else
            {
                TempData["InfoMessage"] = "Bu tarif zaten favorilerinizde.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var favorite = await _context.FavoriteRecipes
                .FirstOrDefaultAsync(fr => fr.UserId == currentUser.Id && fr.RecipeId == id);

            if (favorite != null)
            {
                _context.FavoriteRecipes.Remove(favorite);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tarif favorilerinizden çıkarıldı.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var favoriteRecipes = await _context.FavoriteRecipes
                .Where(fr => fr.UserId == currentUser.Id)
                .Include(fr => fr.Recipe)
                    .ThenInclude(r => r.Category)
                .Include(fr => fr.Recipe)
                    .ThenInclude(r => r.RecipeTags)
                        .ThenInclude(rt => rt.Tag)
                .OrderByDescending(fr => fr.CreatedAt)
                .Select(fr => fr.Recipe)
                .ToListAsync();

            return View(favoriteRecipes);
        }
    }
}
