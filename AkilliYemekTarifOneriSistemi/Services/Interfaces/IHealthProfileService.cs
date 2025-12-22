using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IHealthProfileService
    {
        
        Task<UserProfile?> GetProfileAsync(string userId);
        Task<UserProfile?> GetOrCreateProfileAsync(string userId);


        
        Task<double?> GetTargetCaloriesAsync(string userId);
        double CalculateMaintenanceCalories(UserProfile profile);

        
        double CalculateBMI(double heightCm, double weightKg);
        string GetBMICategory(double bmi);

        
        Task<(double proteinGr, double fatGr, double carbGr)?> GetMacroTargetsAsync(string userId);
    }
}
