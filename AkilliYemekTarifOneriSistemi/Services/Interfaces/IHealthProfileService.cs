using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IHealthProfileService
    {
        // Profil
        Task<UserProfile?> GetProfileAsync(string userId);

        // Kalori
        Task<double?> GetTargetCaloriesAsync(string userId);
        double CalculateMaintenanceCalories(UserProfile profile);

        // BMI
        double CalculateBMI(double heightCm, double weightKg);
        string GetBMICategory(double bmi);

        // Makrolar
        Task<(double proteinGr, double fatGr, double carbGr)?> GetMacroTargetsAsync(string userId);
    }
}
