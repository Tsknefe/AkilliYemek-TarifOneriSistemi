using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    public class HealthProfileService : IHealthProfileService
    {
        private readonly ApplicationDbContext _context;

        public HealthProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<UserProfile?> GetProfileAsync(string userId)
        {
            return await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<UserProfile> GetOrCreateProfileAsync(string userId)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
            if (profile != null) return profile;

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
            return profile;
        }

        
        public double CalculateBMI(double heightCm, double weightKg)
        {
            var heightMeter = heightCm / 100.0;
            return Math.Round(weightKg / (heightMeter * heightMeter), 1);
        }

        public string GetBMICategory(double bmi)
        {
            if (bmi < 18.5) return "Zayýf";
            if (bmi < 25) return "Normal";
            if (bmi < 30) return "Fazla Kilolu";
            return "Obez";
        }

        private static string NormalizeGender(string? gender)
            => (gender ?? "").Trim().ToLowerInvariant();

        private static string NormalizeActivityLevel(string? level)
            => (level ?? "").Trim().ToLowerInvariant();

        private static string NormalizeGoal(string? goal)
            => (goal ?? "").Trim().ToLowerInvariant();

        private double CalculateBmr(UserProfile p)
        {
            var g = NormalizeGender(p.Gender);
            bool isMale = g == "male" || g == "erkek";

            if (isMale)
                return 10 * p.WeightKg + 6.25 * p.HeightCm - 5 * p.Age + 5;

            return 10 * p.WeightKg + 6.25 * p.HeightCm - 5 * p.Age - 161;
        }

        private double GetActivityMultiplier(string? level)
        {
            var l = NormalizeActivityLevel(level);

            return l switch
            {
                "sedentary" => 1.2,
                "light" => 1.375,
                "moderate" => 1.55,
                "active" => 1.725,
                "athlete" => 1.9,
                _ => 1.2
            };
        }

        public double CalculateMaintenanceCalories(UserProfile profile)
        {
            double bmr = CalculateBmr(profile);
            double multiplier = GetActivityMultiplier(profile.ActivityLevel);
            return Math.Round(bmr * multiplier);
        }

        public async Task<double?> GetTargetCaloriesAsync(string userId)
        {
            var profile = await GetProfileAsync(userId);
            if (profile == null)
                return null;

            double maintenance = CalculateMaintenanceCalories(profile);
            var goal = NormalizeGoal(profile.Goal);

            return goal switch
            {
                "lose" => maintenance - 300,
                "gain" => maintenance + 300,
                _ => maintenance
            };
        }
        public async Task<(double proteinGr, double fatGr, double carbGr)?> GetMacroTargetsAsync(string userId)
        {
            var calories = await GetTargetCaloriesAsync(userId);
            if (!calories.HasValue)
                return null;

            double total = calories.Value;

            double proteinCal = total * 0.25;
            double fatCal = total * 0.30;
            double carbCal = total * 0.45;

            return (
                proteinGr: Math.Round(proteinCal / 4, 1),
                fatGr: Math.Round(fatCal / 9, 1),
                carbGr: Math.Round(carbCal / 4, 1)
            );
        }
    }
}
