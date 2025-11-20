using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    public class IngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // LIST
        // ----------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var list = await _context.Ingredients.ToListAsync();
            return View(list);
        }

        // ----------------------------------------------------
        // CREATE (GET)
        // ----------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }

        // ----------------------------------------------------
        // CREATE (POST)
        // ----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingredient ingredient)
        {
            // 🟢 Güvenli tarafta olmak için formdan kendimiz çekiyoruz
            var englishNameFromForm = Request.Form["EnglishName"].ToString();
            if (!string.IsNullOrWhiteSpace(englishNameFromForm))
            {
                ingredient.EnglishName = englishNameFromForm;
            }

            if (ModelState.IsValid)
            {
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ingredient);
        }

        // ----------------------------------------------------
        // EDIT (GET)
        // ----------------------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
                return NotFound();

            return View(ingredient);
        }

        // ----------------------------------------------------
        // EDIT (POST)
        // ----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ingredient ingredient)
        {
            if (id != ingredient.Id)
                return NotFound();

            // Yine garantiye alalım
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Ingredients.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(ingredient);
        }

        // ----------------------------------------------------
        // DELETE (GET)
        // ----------------------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ingredient == null)
                return NotFound();

            return View(ingredient);
        }

        // ----------------------------------------------------
        // DELETE (POST)
        // ----------------------------------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
