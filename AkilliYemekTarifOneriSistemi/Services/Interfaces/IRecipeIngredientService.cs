using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IRecipeIngredientService
    {
        Task<List<RecipeIngredient>> GetByRecipeIdAsync(int recipeId);
        Task<RecipeIngredient?> GetByIdAsync(int id);

        Task<RecipeIngredient> AddAsync(int recipeId, int ingredientId, double quantity, string unit);
        Task<RecipeIngredient?> UpdateAsync(int id, int ingredientId, double quantity, string unit);
        Task<bool> DeleteAsync(int id);
    }
}
