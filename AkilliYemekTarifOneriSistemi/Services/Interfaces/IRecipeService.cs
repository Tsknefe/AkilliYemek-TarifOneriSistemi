using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetAllAsync(string search = null);
        Task<Recipe?> GetByIdAsync(int id);
        Task<Recipe> CreateAsync(Recipe recipe);
        Task<Recipe> UpdateAsync(Recipe recipe);
        Task<Recipe> DeleteAsync(int id);
    }
}
