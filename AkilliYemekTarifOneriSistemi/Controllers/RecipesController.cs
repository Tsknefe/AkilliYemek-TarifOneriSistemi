using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    //Burada normal CRUD işlemlerini tanımladığımız yer
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;


        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get:/recipes
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
            var recipes=await query.AsNoTracking().ToListAsync();
            ViewBag.Search=search;
            return View(recipes);

        }

        //Get:/recipes/details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var recipe = await _context.Recipes
                .Include(r => r.NutritionFacts)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
                return NotFound();

            return View(recipe);

        }

        //get:/recipes/create
        public IActionResult Create()
        {
            return View();
        }

        //Post:/recipes/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,CookingTime,Servings,DietType")]
            Recipe recipe)
        {
            if (!ModelState.IsValid)
                return View(recipe);

            _context.Add(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Get:/recipes/edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        //Post:/recipes/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Description,CookingTime,Servings,DietType")]
            Recipe recipe)
        {
            if (id != recipe.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(recipe);

            try
            {
                _context.Update(recipe);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(recipe.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        //Get:/recipes/Delete/5
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

        //Post:/recipes/Delete/5
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
