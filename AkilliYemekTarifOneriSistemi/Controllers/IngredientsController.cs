using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    // bu controller mvc tarafında malzeme CRUD işlemlerini yönettiğimiz yer
    // admin panel gibi düşün buradan malzeme ekleme silme güncelleme yapıyoruz
    // api ile karıştırmamak lazım bu razor view dönen klasik mvc controller
    public class IngredientsController : Controller
    {
        // veritabanı context
        private readonly ApplicationDbContext _context;

        public IngredientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // liste sayfası
        // tüm malzemeleri çekip Index viewına gönderiyoruz
        public async Task<IActionResult> Index()
        {
            var list = await _context.Ingredients.ToListAsync();
            return View(list);
        }

        // yeni malzeme ekleme formunu gösteren get action
        public IActionResult Create()
        {
            return View();
        }

        // create post kısmı form submit edilince buraya düşüyor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingredient ingredient)
        {
            // englishname için formdan manuel çekiyoruz
            // bazı durumlarda modelbinding kaçırabiliyor o yüzden garantiye alıyoruz
            var englishNameFromForm = Request.Form["EnglishName"].ToString();
            if (!string.IsNullOrWhiteSpace(englishNameFromForm))
            {
                ingredient.EnglishName = englishNameFromForm;
            }

            // model valid ise veritabanına kaydediyoruz
            if (ModelState.IsValid)
            {
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // hata varsa aynı formu bu sefer validasyon mesajlarıyla geri gösteriyoruz
            return View(ingredient);
        }

        // düzenleme sayfasının get kısmı
        // id ye göre malzemeyi bulup edit viewına gönderiyoruz
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
                return NotFound();

            return View(ingredient);
        }

        // edit post kısmı form kaydedilince buraya geliyor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ingredient ingredient)
        {
            // url deki id ile modeldeki id aynı olmalı
            if (id != ingredient.Id)
                return NotFound();

            // yine englishname i formdan ayrıca çekiyoruz
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
                    // bu esnada kayıt silinmiş olabilir o yüzden existence kontrolü yapıyoruz
                    if (!_context.Ingredients.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // validasyon patlarsa formu tekrar gösteriyoruz
            return View(ingredient);
        }

        // silme onay sayfasının get kısmı
        // önce kullanıcıya emin misin diye gösteriyoruz
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

        // silme işleminin post kısmı
        // gerçekten silme burada yapılıyor
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

            // silindikten sonra tekrar listeye dönüyoruz
            return RedirectToAction(nameof(Index));
        }
    }
}
