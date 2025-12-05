using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    /// <summary>
    /// Tarif (Recipe) listeleme ve detay gösterme işlemlerini yöneten MVC controller.
    /// Burası kullanıcı tarafındaki arayüz:
    /// - Tarifleri kart/grid şeklinde listeleme
    /// - Tarif detay sayfası
    /// - Favorilere ekleme / çıkarma
    /// Admin CRUD kısmı ileride ayrı bir controller olarak eklenebilir.
    /// </summary>
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

        /// <summary>
        /// Tüm tarifleri listeleyen sayfa (Card/Grid görünümü).
        /// GET: /Recipes veya /Recipes/Index
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Tüm tarifleri kategorileri ve etiketleriyle birlikte getiriyoruz.
            var recipes = await _context.Recipes
                .Include(r => r.Category) // Kategori bilgisi için
                .Include(r => r.RecipeTags)
                    .ThenInclude(rt => rt.Tag) // Etiket bilgileri için
                .OrderByDescending(r => r.Id) // En yeni tarifler üstte
                .ToListAsync();

            return View(recipes);
        }

        /// <summary>
        /// Belirli bir tarifin detay sayfasını gösterir.
        /// GET: /Recipes/Details/5
        /// </summary>
        /// <param name="id">Tarifin Id değeri</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tarifi tüm ilişkili verilerle birlikte getiriyoruz:
            // - Kategori bilgisi
            // - Etiketler
            // - Malzemeler (RecipeIngredients) ve Ingredient detayları
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

            // Kullanıcı giriş yapmışsa, bu tarifi favorilerine eklemiş mi kontrol et
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

        /// <summary>
        /// Kullanıcının favorilerine tarif ekler.
        /// POST: /Recipes/AddFavorite/5
        /// </summary>
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

            // Zaten favoride mi kontrol et
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

        /// <summary>
        /// Kullanıcının favorilerinden tarifi çıkarır.
        /// POST: /Recipes/RemoveFavorite/5
        /// </summary>
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

        /// <summary>
        /// Kullanıcının favori tariflerini listeler.
        /// GET: /Recipes/Favorites
        /// </summary>
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
