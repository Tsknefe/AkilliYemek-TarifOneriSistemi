using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    // bu controller haftalık beslenme planı sayfasını yönetiyor
    // kullanıcı kendi profil bilgilerine göre otomatik plan oluşturabiliyor
    // kullanıcı giriş yapmadan bu sayfaya erişemiyor çünkü kişisel plan üretimi gerekiyor
    [Authorize]
    public class WeeklyPlanController : Controller
    {
        private readonly IWeeklyPlanService _weeklyPlanService;

        // servis dışarıdan geliyor böylece hem test edilebilir hem bağımlılık azaltılmış oluyor
        public WeeklyPlanController(IWeeklyPlanService weeklyPlanService)
        {
            _weeklyPlanService = weeklyPlanService;
        }

        // kullanıcı Id sini identity üzerinden alıyoruz
        private string? GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        // haftalık plan ekranı GET metodu
        // startDate verilirse o haftadan başlıyor verilmezse bugünden itibaren devam ediyor
        public async Task<IActionResult> Index(DateTime? startDate = null)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            // servis ile asıl plan oluşturma işlemi yapılıyor
            var plan = await _weeklyPlanService.GenerateWeeklyPlanAsync(userId, startDate);

            // plan null dönerse genelde tarif yok ya da kullanıcı profili eksik olduğu için
            if (plan == null)
            {
                ViewBag.Error = "Plan oluşturulamadı. Lütfen profil bilgilerinizi doldurduğunuzdan ve yeterli tarif olduğundan emin olun.";
                return View(null);
            }

            // planı view a gönderiyoruz model WeeklyPlanDto
            return View(plan);
        }
    }
}
