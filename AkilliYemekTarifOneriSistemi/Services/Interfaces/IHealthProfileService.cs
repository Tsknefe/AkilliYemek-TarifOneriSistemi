using AkilliYemekTarifOneriSistemi.Models;

public interface IHealthProfileService
{
    Task<double?> GetTargetCaloriesAsync(string userId);
    Task<(double proteinGr, double fatGr, double carbGr)?> GetMacroTargetsAsync(string userId);
    Task<UserProfile?> GetProfileAsync(string userId);
}
