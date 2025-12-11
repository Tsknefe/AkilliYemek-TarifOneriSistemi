using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    /// <summary>
    /// Malzeme (Ingredient) CRUD işlemlerini yöneten MVC controller.
    /// Burası admin paneli gibi düşünülebilir; Razor View döner, API değildir.
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
            // Tüm malzemeleri ada göre sıralı çekiyoruz
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

            // Malzemenin kullanıldığı tarifleri de dahil ederek getiriyoruz
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
        public async Task<IActionResult> Create(Ingredient ingredient)
        {
            // EnglishName için formdan manuel çekiyoruz.
            // Bazı durumlarda model binding kaçırabiliyor, garantiye alıyoruz.
            var englishNameFromForm = Request.Form["EnglishName"].ToString();
            if (!string.IsNullOrWhiteSpace(englishNameFromForm))
            {
                ingredient.EnglishName = englishNameFromForm;
            }

            if (ModelState.IsValid)
            {
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Malzeme başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }

            // Validasyon hatası varsa formu aynı model ile geri gösteriyoruz
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
                return NotFound();

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
                return NotFound();

            return View(ingredient);
        }

        /// <summary>
        /// Malzeme düzenleme işlemini gerçekleştirir.
        /// POST: /Ingredients/Edit/5
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ingredient ingredient)
        {
            // URL'deki id ile modeldeki id aynı olmalı
            if (id != ingredient.Id)
                return NotFound();

            // EnglishName için yine formdan manuel çekiyoruz
            var englishNameFromForm = Request.Form["EnglishName"].ToString();
            if (!string.IsNullOrWhiteSpace(englishNameFromForm))
            {
                ingredient.EnglishName = englishNameFromForm;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Malzeme başarıyla güncellendi!";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!IngredientExists(ingredient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Ingredient güncellenirken concurrency hatası oluştu. Id={Id}", ingredient.Id);
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // Validasyon patlarsa formu tekrar gösteriyoruz
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
                return NotFound();

            var ingredient = await _context.Ingredients
                .Include(i => i.RecipeIngredients)
                    .ThenInclude(ri => ri.Recipe)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
                return NotFound();

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
                // Malzeme tariflerde kullanılıyorsa, ilişkiler cascade delete ile temizlenecek
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Malzeme başarıyla silindi!";
            }

            // Silindikten sonra tekrar listeye dönüyoruz
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
