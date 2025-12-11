using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    // bu controller tamamen kullanıcının alerji listesini yönetmek için
    // yani kullanıcı giriş yapıyor ve kendi alerjilerini ekleyip silebiliyor
    // api değil razor view döndüren klasik mvc controller
    // authorize koyduk çünkü kullanıcı girişi zorunlu
    [Authorize]
    public class UserAllergiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAllergyService _allergyService;

        public UserAllergiesController(
            ApplicationDbContext context,
            IAllergyService allergyService)
        {
            _context = context;
            _allergyService = allergyService;
        }

        // helper fonksiyon user id alma
        // login olmuş kullanıcının IdentityUser Id sini alıyoruz
        private string? GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        // alerji listesi sayfası
        // kullanıcı kendi eklediği alerjileri burada görüyor
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            // alerjileri service üzerinden çekiyoruz
            var list = await _allergyService.GetUserAllergiesAsync(userId);

            return View(list);
        }

        // yeni alerji ekleme ekranı GET
        // malzeme listesini dropdownda göstermek için Ingredients gönderiyoruz
        public async Task<IActionResult> Create()
        {
            var ingredients = await _context.Ingredients.ToListAsync();
            ViewBag.Ingredients = ingredients;
            return View();
        }

        // yeni alerji ekleme POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            // kullanıcı malzeme seçmeden gönderirse hata veriyoruz
            if (ingredientId == 0)
            {
                ModelState.AddModelError("", "Lütfen bir malzeme seçin");
                ViewBag.Ingredients = await _context.Ingredients.ToListAsync();
                return View();
            }

            // service üzerinden alerji ekleme işlemi
            await _allergyService.AddAllergyAsync(userId, ingredientId);

            return RedirectToAction(nameof(Index));
        }

        // alerji silme ekranı GET
        // sadece silmeden önce kullanıcıya emin misin göstermek için
        public async Task<IActionResult> Delete(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            // kullanıcıya ait tüm alerjileri çekiyoruz
            var list = await _allergyService.GetUserAllergiesAsync(userId);

            // silinecek olanı listeden buluyoruz
            var item = list.FirstOrDefault(x => x.IngredientId == ingredientId);
            if (item == null) return NotFound();

            return View(item);
        }

        // silme işlemi POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            // service ile silme yapıyoruz
            await _allergyService.RemoveAllergyAsync(userId, ingredientId);

            return RedirectToAction(nameof(Index));
        }
    }
}
