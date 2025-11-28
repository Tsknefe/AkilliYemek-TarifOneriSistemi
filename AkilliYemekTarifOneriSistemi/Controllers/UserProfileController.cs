using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    // bu controller kullanıcının kendi profilini yönettiği yer
    // yaş boy kilo aktivite seviyesi diyet tipi gibi bilgiler burada tutuluyor
    // öneri motoru bu bilgileri kullanarak daha kişisel sonuç üretiyor
    // authorize olduğu için giriş yapmadan erişim mümkün değil
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // login olmuş kullanıcının Id sini claimden çekiyoruz
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // profil düzenleme ekranı GET
        // kullanıcı ilk defa geliyorsa otomatik olarak default bir profil oluşturuluyor
        public async Task<IActionResult> Edit()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // ilgili kullanıcı profil tablosunda var mı kontrol ediyoruz
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

            // profil yoksa default değerlerle yeni bir profil oluşturuyoruz
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    Age = 25,
                    HeightCm = 170,
                    WeightKg = 70,
                    Gender = "Male",
                    ActivityLevel = "sedentary",
                    Goal = "Maintain",
                    DietType = "Normal"
                };

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            return View(profile);
        }

        // profil düzenleme POST
        // viewdan gelen model ile veritabanındaki profil güncelleniyor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfile model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // veritabanındaki gerçek kaydı çekiyoruz
            var existing = await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existing == null)
                return NotFound();

            // validasyon hatası olursa tekrar formu gösteriyoruz
            if (!ModelState.IsValid)
                return View(model);

            // kullanıcı formda ne girdiyse o değerleri profile aktarıyoruz
            existing.Age = model.Age;
            existing.HeightCm = model.HeightCm;
            existing.WeightKg = model.WeightKg;
            existing.Gender = model.Gender;
            existing.ActivityLevel = model.ActivityLevel;
            existing.Goal = model.Goal;
            existing.DietType = model.DietType;

            await _context.SaveChangesAsync();

            // kullanıcıya başarılı olduğuna dair mesaj gösterebilmek için
            ViewBag.Message = "Profiliniz başarıyla güncellendi";

            return View(existing);
        }
    }
}
