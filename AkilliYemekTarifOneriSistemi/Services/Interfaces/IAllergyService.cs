using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IAllergyService
    {
        Task<List<int>> GetUserAllergyIngredientIdsAsync(string userId);
        Task<List<UserAllergy>> GetUserAllergiesAsync(string userId);

        Task AddAllergyAsync(string userId, int ingredientId);
        Task RemoveAllergyAsync(string userId, int ingredientId);
    }
}
