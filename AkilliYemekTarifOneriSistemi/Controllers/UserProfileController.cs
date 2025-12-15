using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHealthProfileService _healthService;

        public UserProfileController(
            ApplicationDbContext context,
            IHealthProfileService healthService)
        {
            _context = context;
            _healthService = healthService;
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // ============================
        // INDEX – PROFİL ÖZETİ
        // ============================
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (profile == null)
                return RedirectToAction(nameof(Edit));

            FillHealthViewBags(profile, userId);

            return View(profile);
        }

        // ============================
        // GET – PROFİL DÜZENLE
        // ============================
        public async Task<IActionResult> Edit()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

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

        // ============================
        // POST – PROFİL GÜNCELLE
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfile model)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var existing = await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            existing.Age = model.Age;
            existing.HeightCm = model.HeightCm;
            existing.WeightKg = model.WeightKg;
            existing.Gender = model.Gender;
            existing.ActivityLevel = model.ActivityLevel;
            existing.Goal = model.Goal;
            existing.DietType = model.DietType;

            // 🔥 EF CORE ZORLA UPDATE
            _context.Entry(existing).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi";

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // HESAPLAMALAR
        // ============================
        private void FillHealthViewBags(UserProfile profile, string userId)
        {
            var bmi = _healthService.CalculateBMI(profile.HeightCm, profile.WeightKg);
            var bmiCategory = _healthService.GetBMICategory(bmi);
            var maintenance = _healthService.CalculateMaintenanceCalories(profile);
            var target = _healthService.GetTargetCaloriesAsync(userId).Result;

            ViewBag.BMI = Math.Round(bmi, 1);
            ViewBag.BMICategory = bmiCategory;
            ViewBag.MaintenanceCalories = Math.Round(maintenance);
            ViewBag.TargetCalories = Math.Round(target ?? maintenance);
        }
    }
}
