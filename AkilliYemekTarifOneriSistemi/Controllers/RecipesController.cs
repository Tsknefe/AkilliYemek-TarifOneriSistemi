using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /recipes
        [AllowAnonymous]
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Recipes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e =>
                    e.Name.Contains(search) ||
                    e.Description.Contains(search) ||
                    e.DietType.Contains(search)
                );
            }

            var recipes = await query.AsNoTracking().ToListAsync();
            ViewBag.Search = search;

            return View(recipes);
        }

        // GET: /recipes/details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var recipe = await _context.Recipes
                .Include(r => r.NutritionFacts)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        // GET: /recipes/create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /recipes/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,CookingTime,Servings,DietType")] Recipe recipe,
            IFormFile? imageFile)
        {
            Console.WriteLine(">>> CREATE POST METODU ÇALIŞTI");

            if (!ModelState.IsValid)
            {
                Console.WriteLine(">>> MODELSTATE INVALID");
                return View(recipe);
            }

            // ⭐ Resim yükleme
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                recipe.ImagePath = "/uploads/" + fileName;
            }

            _context.Add(recipe);
            await _context.SaveChangesAsync();

            Console.WriteLine(">>> KAYIT BAŞARILI ✔");

            return RedirectToAction(nameof(Index));
        }

        // GET: /recipes/edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        // POST: /recipes/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Description,CookingTime,Servings,DietType,ImagePath")]
            Recipe recipe,
            IFormFile? newImage)
        {
            if (id != recipe.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(recipe);

            // ⭐ Resim değiştirme (opsiyonel)
            if (newImage != null && newImage.Length > 0)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(newImage.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newImage.CopyToAsync(stream);
                }

                recipe.ImagePath = "/uploads/" + fileName;
            }

            _context.Update(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /recipes/delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var recipe = await _context.Recipes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        // POST: /recipes/delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(r => r.Id == id);
        }
    }
}
