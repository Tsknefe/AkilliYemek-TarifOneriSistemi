using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IRecipeIngredientService
    {
        // 📋 Tarife ait tüm malzemeleri getir
        Task<List<RecipeIngredient>> GetByRecipeIdAsync(int recipeId);

        // 🔍 Tek bir tarif–malzeme kaydını getir
        Task<RecipeIngredient?> GetByIdAsync(int id);

        // ➕ Tarife yeni malzeme ekle
        Task<RecipeIngredient> AddAsync(
            int recipeId,
            int ingredientId,
            double quantity,
            string unit);

        // ✏️ Tarif içindeki malzemeyi güncelle
        Task<RecipeIngredient?> UpdateAsync(
            int id,
            int ingredientId,
            double quantity,
            string unit);

        // ❌ Tarife ait malzemeyi sil
        Task<bool> DeleteAsync(int id);

        // 🔥 API üzerinden BU MALZEMENİN kalorisi (miktar + birime göre)
        Task<double> CalculateCaloriesAsync(int recipeIngredientId);

    }
}
