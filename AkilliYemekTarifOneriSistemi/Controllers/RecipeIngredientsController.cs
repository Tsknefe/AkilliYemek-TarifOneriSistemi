using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using AkilliYemekTarifOneriSistemi.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    // bu controller tariflere ait malzeme yönetimini razor tarafında yaptığımız yer
    // yani admin panelde "tarife malzeme ekleme - düzenleme - silme" işlemlerinin mvc versiyonu
    // api tarafındaki RecipeIngredientApiController ile karıştırmamak lazım
    public class RecipeIngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INutritionService _nutritionService;

        public RecipeIngredientsController(ApplicationDbContext context, INutritionService nutritionService)
        {
            _context = context;
            _nutritionService = nutritionService;
        }

        // ADD GET
        // bu action bir tarife malzeme ekleme formunu göstermek için kullanılıyor
        public async Task<IActionResult> Add(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return NotFound();

            // view tarafında dropdown oluşturabilmek için malzemeleri çekiyoruz
            ViewBag.RecipeId = id;
            ViewBag.Ingredients = await _context.Ingredients.ToListAsync();

            return View();
        }

        // ADD POST
        // form submit edildikten sonra gerçekten malzeme ekleyen kısım
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, int ingredientId, double quantity, string unit)
        {
            // form validation
            if (ingredientId == 0 || quantity <= 0 || string.IsNullOrWhiteSpace(unit))
            {
                ModelState.AddModelError("", "Lütfen tüm alanları doldurun");
                ViewBag.RecipeId = id;
                ViewBag.Ingredients = await _context.Ingredients.ToListAsync();
                return View();
            }

            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null)
                return NotFound();

            // gram dönüşümü için helper kullanıyoruz
            double grams = UnitConverter.ToGram(quantity, unit, ingredient.Name);

            // yeni tarif-malzeme ilişkisi oluşturuyoruz
            var ri = new RecipeIngredient
            {
                RecipeId = id,
                IngredientId = ingredientId,
                Quantity = quantity.ToString(),
                Unit = unit,
                CalculatedGrams = grams
            };

            _context.RecipeIngredients.Add(ri);
            await _context.SaveChangesAsync();

            // malzeme değiştiğinde besin hesabını güncelliyoruz
            await _nutritionService.SaveNutritionForRecipeAsync(id);

            return RedirectToAction("Details", "Recipes", new { id });
        }

        // EDIT GET
        // bir tarifteki mevcut bir malzeme satırını düzenleme formuna getiriyoruz
        public async Task<IActionResult> Edit(int id)
        {
            var ri = await _context.RecipeIngredients
                .Include(x => x.Ingredient)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ri == null)
                return NotFound();

            // eksik olan kısım burasıydı dropdown için tüm malzemeleri gönderiyoruz
            ViewBag.Ingredients = await _context.Ingredients.ToListAsync();

            return View(ri);
        }

        // EDIT POST
        // düzenleme formu kaydedilince burası çalışıyor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, double quantity, string unit, int ingredientId)
        {
            var ri = await _context.RecipeIngredients
                .Include(r => r.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ri == null)
                return NotFound();

            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null)
                return NotFound();

            // gram hesaplamayı tekrar yapıyoruz
            double grams = UnitConverter.ToGram(quantity, unit, ingredient.Name);

            // alanları güncelliyoruz
            ri.IngredientId = ingredientId;
            ri.Quantity = quantity.ToString();
            ri.Unit = unit;
            ri.CalculatedGrams = grams;

            await _context.SaveChangesAsync();

            // besin değerini güncelle
            await _nutritionService.SaveNutritionForRecipeAsync(ri.RecipeId);

            return RedirectToAction("Details", "Recipes", new { id = ri.RecipeId });
        }

        // DELETE GET
        // kullanıcıya "malzemeyi silmek istiyor musun" ekranını gösteriyoruz
        public async Task<IActionResult> Delete(int id)
        {
            var ri = await _context.RecipeIngredients
                .Include(x => x.Ingredient)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ri == null)
                return NotFound();

            return View(ri);
        }

        // DELETE POST
        // silme işlemi burada yapılır
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ri = await _context.RecipeIngredients.FindAsync(id);
            if (ri == null)
                return NotFound();

            int recipeId = ri.RecipeId;

            // ilişkili malzeme kaydını siliyoruz
            _context.RecipeIngredients.Remove(ri);
            await _context.SaveChangesAsync();

            // besin değerini tekrar güncelle
            await _nutritionService.SaveNutritionForRecipeAsync(recipeId);

            return RedirectToAction("Details", "Recipes", new { id = recipeId });
        }
    }
}
