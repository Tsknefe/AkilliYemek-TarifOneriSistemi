using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    /// <summary>
    /// Malzeme (Ingredient) CRUD işlemlerini yöneten controller.
    /// Melisa'nın sorumluluğu: Ingredient CRUD ekranları.
    /// </summary>
    public class IngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IngredientsController> _logger;

        public IngredientsController(ApplicationDbContext context, ILogger<IngredientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tüm malzemeleri listeleyen sayfa.
        /// GET: /Ingredients veya /Ingredients/Index
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var ingredients = await _context.Ingredients
                .OrderBy(i => i.Name)
                .ToListAsync();

            return View(ingredients);
        }

        /// <summary>
        /// Belirli bir malzemenin detay sayfasını gösterir.
        /// GET: /Ingredients/Details/5
        /// </summary>
        /// <param name="id">Malzemenin Id değeri</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.RecipeIngredients)
                    .ThenInclude(ri => ri.Recipe)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        /// <summary>
        /// Yeni malzeme ekleme formunu gösterir.
        /// GET: /Ingredients/Create
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Yeni malzeme ekleme işlemini gerçekleştirir.
        /// POST: /Ingredients/Create
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,DefaultUnit,Description")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ingredient);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Malzeme başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }

        /// <summary>
        /// Malzeme düzenleme formunu gösterir.
        /// GET: /Ingredients/Edit/5
        /// </summary>
        /// <param name="id">Malzemenin Id değeri</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }

        /// <summary>
        /// Malzeme düzenleme işlemini gerçekleştirir.
        /// POST: /Ingredients/Edit/5
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DefaultUnit,Description")] Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Malzeme başarıyla güncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }

        /// <summary>
        /// Malzeme silme onay sayfasını gösterir.
        /// GET: /Ingredients/Delete/5
        /// </summary>
        /// <param name="id">Malzemenin Id değeri</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.RecipeIngredients)
                    .ThenInclude(ri => ri.Recipe)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        /// <summary>
        /// Malzeme silme işlemini gerçekleştirir.
        /// POST: /Ingredients/Delete/5
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                // Malzeme tariflerde kullanılıyorsa, önce RecipeIngredients kayıtları silinir (Cascade delete).
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Malzeme başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Belirli bir Id'ye sahip malzemenin var olup olmadığını kontrol eder.
        /// </summary>
        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }
    }
}

