using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Models.ViewModels;
using AkilliYemekTarifOneriSistemi.Services.DietRules;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHealthProfileService _healthService;
        private readonly UserManager<IdentityUser> _userManager;

        public UserProfileController(
            ApplicationDbContext context,
            IHealthProfileService healthService,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _healthService = healthService;
            _userManager = userManager;
        }

        private string? GetCurrentUserId() => _userManager.GetUserId(User); 

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
            if (profile == null) return RedirectToAction(nameof(Edit));

            await FillHealthViewBagsAsync(profile, userId);
            return View(profile);
        }

        
        public async Task<IActionResult> Edit()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);

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
                    DietType = "normal"
                };

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            var vm = new UserProfileEditViewModel
            {
                Age = profile.Age,
                HeightCm = profile.HeightCm,
                WeightKg = profile.WeightKg,
                Gender = profile.Gender,
                ActivityLevel = profile.ActivityLevel,
                Goal = profile.Goal,
                DietType = profile.DietType
            };

            return View(vm);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileEditViewModel model)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            if (!ModelState.IsValid)
                return View(model);

            var existing = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
            if (existing == null) return NotFound();

            existing.Age = model.Age;
            existing.HeightCm = model.HeightCm;
            existing.WeightKg = model.WeightKg;
            existing.Gender = model.Gender;
            existing.ActivityLevel = model.ActivityLevel;
            existing.Goal = model.Goal;

            existing.DietType = DietTypeNormalizer.Normalize(model.DietType);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profiliniz baþarýyla güncellendi";
            return RedirectToAction(nameof(Index));
        }

        private async Task FillHealthViewBagsAsync(UserProfile profile, string userId)
        {
            var bmi = _healthService.CalculateBMI(profile.HeightCm, profile.WeightKg);
            var bmiCategory = _healthService.GetBMICategory(bmi);
            var maintenance = _healthService.CalculateMaintenanceCalories(profile);

            var target = await _healthService.GetTargetCaloriesAsync(userId);

            ViewBag.BMI = Math.Round(bmi, 1);
            ViewBag.BMICategory = bmiCategory;
            ViewBag.MaintenanceCalories = Math.Round(maintenance);
            ViewBag.TargetCalories = Math.Round(target ?? maintenance);
        }
    }
}
