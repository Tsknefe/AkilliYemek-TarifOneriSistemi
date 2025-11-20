using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> GetAllAsync();
        Task<Ingredient> GetByIdAsync(int id);
        Task<Ingredient> CreateAsync(Ingredient ingredient);
    }
}
