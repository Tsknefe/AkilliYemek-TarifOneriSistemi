using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    // burada tarifler için tam kapsamlı CRUD işlemlerini yapan MVC controller var
    // yani admin panel tarafındaki “tarif ekleme - düzenleme - silme - listeleme” işlemleri buradan yönetiliyor
    // api controller ile karıştırılmamalı çünkü bu taraf sadece razor view döndürüyor
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX (Liste)
        // tarifleri listeliyoruz, eğer search dolu ise filtre uyguluyoruz
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Recipes.AsQueryable();

            // arama özelliği
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e =>
                    e.Name.Contains(search) ||
                    e.Description.Contains(search) ||
                    e.DietType.Contains(search)
                );
            }

            var recipes = await query.AsNoTracking().ToListAsync();

            // view tarafında textbox içinde arama terimini gösterebilmek için
            ViewBag.Search = search;

            return View(recipes);
        }

        // DETAILS
        // tarifin tüm detaylarını getiriyoruz
        // besin değerleri, malzemeleri ve malzemelerin Ingredient navigationları dahil
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

        // CREATE GET
        // formu göstermek için
        public IActionResult Create()
        {
            return View();
        }

        // CREATE POST
        // yeni tarif oluşturma işlemi burada yapılıyor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,CookingTime,Servings,DietType,Instructions")]
            Recipe recipe)
        {
            Console.WriteLine(">>> CREATE POST METODU ÇALIŞTI");

            // validasyon kontrolü
            if (!ModelState.IsValid)
            {
                Console.WriteLine(">>> MODELSTATE INVALID (Geçersiz)");

                foreach (var err in ModelState)
                {
                    if (err.Value.Errors.Count > 0)
                        Console.WriteLine($"Hata: {err.Key} => {err.Value.Errors[0].ErrorMessage}");
                }

                return View(recipe);
            }

            Console.WriteLine(">>> MODELSTATE VALID (Geçerli)");

            try
            {
                // veritabanına ekleme
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                Console.WriteLine(">>> KAYIT BAŞARILI 🔥🔥🔥");
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>> HATA OLDU ❌: " + ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        // tarif düzenleme formunu malzemeleriyle birlikte getiriyoruz
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        // EDIT POST
        // tarif bilgilerini güncellediğimiz yer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Description,CookingTime,Servings,DietType,Instructions")]
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

        // DELETE GET
        // silme ekranını göstermek için
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

        // DELETE POST
        // gerçekten silme işlemi burada yapılıyor
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

        // helper fonksiyon
        // tarif veritabanında var mı diye kontrol ediyor
        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(r => r.Id == id);
        }
    }
}
