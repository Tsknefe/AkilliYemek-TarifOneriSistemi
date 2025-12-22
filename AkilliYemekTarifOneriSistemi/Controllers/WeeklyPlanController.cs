using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Services.DietRules;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    [Authorize]
    public class WeeklyPlanController : Controller
    {
        private readonly IWeeklyPlanService _weeklyPlanService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public WeeklyPlanController(
            IWeeklyPlanService weeklyPlanService,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _weeklyPlanService = weeklyPlanService;
            _userManager = userManager;
            _context = context;
        }

        private string? GetUserId() => _userManager.GetUserId(User);

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate = null)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var start = (startDate ?? DateTime.Today).Date;

            var profile = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            var currentDiet = DietTypeNormalizer.Normalize(profile?.DietType);

            
            var existing = await _context.WeeklyPlans
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.StartDate.Date == start.Date)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                var oldDiet = DietTypeNormalizer.Normalize(existing.DietTypeSnapshot);

                if (oldDiet != currentDiet)
                {
                    ViewBag.Warning = $"Profil diyet türünüz deðiþti ({oldDiet} › {currentDiet}). Plan yeniden oluþturuldu. Kaydet’e basarak güncelleyebilirsiniz.";
                }
            }

            var plan = await _weeklyPlanService.GenerateWeeklyPlanAsync(userId, start);

            if (plan == null)
                ViewBag.Error = "Plan oluþturulamadý. Profil bilgilerinizi doldurun.";

            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(DateTime startDate)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var dto = await _weeklyPlanService.GenerateWeeklyPlanAsync(userId, startDate);
            if (dto == null)
            {
                TempData["ErrorMessage"] = "Plan kaydedilemedi. Profil bilgilerinizi kontrol edin.";
                return RedirectToAction(nameof(Index), new { startDate });
            }

            int id = await _weeklyPlanService.SaveOrReplaceGeneratedPlanAsync(userId, dto);

            TempData["SuccessMessage"] = "Otomatik plan kaydedildi/güncellendi.";
            return RedirectToAction("Details", "WeeklyPlans", new { id });
        }
    }
}
