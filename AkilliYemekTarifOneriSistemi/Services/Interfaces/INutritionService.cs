using System.Threading.Tasks;
using AkilliYemekTarifOneriSistemi.Models;

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface INutritionService
    {
        Task<NutritionFacts> GetNutritionAsync(string name, string englishName = null);
        Task<NutritionFacts> SaveNutritionForRecipeAsync(int recipeId);
    }
}
