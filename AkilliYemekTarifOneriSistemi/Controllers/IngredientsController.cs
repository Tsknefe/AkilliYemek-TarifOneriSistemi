using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    public class IngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IngredientsController> _logger;

        public IngredientsController(ApplicationDbContext context, ILogger<IngredientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var ingredients = await _context.Ingredients
                .OrderBy(i => i.Name)
                .ToListAsync();

            return View(ingredients);
        }

        
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

        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingredient ingredient)
        {
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

            return View(ingredient);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
                return NotFound();

            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ingredient ingredient)
        {
            if (id != ingredient.Id)
                return NotFound();

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

            return View(ingredient);
        }

        
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Malzeme başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }
    }
}
