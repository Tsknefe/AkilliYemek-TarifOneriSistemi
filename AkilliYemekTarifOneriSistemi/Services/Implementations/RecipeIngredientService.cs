using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Helpers;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    // burası tariflere bağlı malzeme yönetiminin olduğu servis
    // yani hem tarif için malzeme ekleme silme güncelleme işlerini hem de besin değerini güncelleme işini burada yapıyoruz
    // controller sadece bu servisi çağırıyor bütün mantık akışı tamamen burada dönüyor

    public class RecipeIngredientService : IRecipeIngredientService
    {
        private readonly ApplicationDbContext _context;
        private readonly INutritionService _nutritionService;

        public RecipeIngredientService(
            ApplicationDbContext context,
            INutritionService nutritionService)
        {
            _context = context;
            _nutritionService = nutritionService;
        }

        // bir tarifin içindeki tüm malzeme kayıtlarını getiriyoruz
        // edit sayfası ve detay ekranı bunu kullanıyor
        public async Task<List<RecipeIngredient>> GetByRecipeIdAsync(int recipeId)
        {
            return await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .Where(ri => ri.RecipeId == recipeId)
                .ToListAsync();
        }

        // tek bir tarif malzemesini id ile çekmek için
        public async Task<RecipeIngredient?> GetByIdAsync(int id)
        {
            return await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .FirstOrDefaultAsync(ri => ri.Id == id);
        }

        // tarif içine yeni bir malzeme ekliyoruz
        // quantity ve unit geldiği için bunları grama çevirmemiz gerekiyor
        public async Task<RecipeIngredient> AddAsync(int recipeId, int ingredientId, double quantity, string unit)
        {
            // önce eklenen malzeme gerçekten db'de var mı kontrol ediyoruz
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null)
                throw new InvalidOperationException("Ingredient bulunamadı");

            // miktarı grama çeviriyoruz servis mantığının önemli kısmı burası
            double grams = UnitConverter.ToGram(quantity, unit, ingredient.Name);

            // db'ye ekleyeceğimiz kayıt nesnesi
            var entity = new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Quantity = quantity.ToString(),
                Unit = unit,
                CalculatedGrams = grams
            };

            // kaydı ekliyoruz
            _context.RecipeIngredients.Add(entity);
            await _context.SaveChangesAsync();

            // malzeme eklenince tarifin besin değerini de otomatik güncelliyoruz
            await _nutritionService.SaveNutritionForRecipeAsync(recipeId);

            return entity;
        }

        // tarifteki bir malzeme kaydını güncelleme işlemi
        public async Task<RecipeIngredient?> UpdateAsync(int id, int ingredientId, double quantity, string unit)
        {
            // önce ilgili kayıt var mı çekiyoruz
            var entity = await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .FirstOrDefaultAsync(ri => ri.Id == id);

            if (entity == null)
                return null;

            // yeni gelen malzeme id var mı kontrol
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null)
                throw new InvalidOperationException("Ingredient bulunamadı");

            // yeniden gram hesaplama
            double grams = UnitConverter.ToGram(quantity, unit, ingredient.Name);

            // güncelleme işlemleri
            entity.IngredientId = ingredientId;
            entity.Quantity = quantity.ToString();
            entity.Unit = unit;
            entity.CalculatedGrams = grams;

            await _context.SaveChangesAsync();

            // güncelleme sonrası tarifin besin değerini de güncelliyoruz
            await _nutritionService.SaveNutritionForRecipeAsync(entity.RecipeId);

            return entity;
        }

        // tariften bir malzemeyi silme işlemi
        public async Task<bool> DeleteAsync(int id)
        {
            // önce kayıt var mı
            var entity = await _context.RecipeIngredients.FindAsync(id);
            if (entity == null)
                return false;

            int recipeId = entity.RecipeId;

            // kaydı siliyoruz
            _context.RecipeIngredients.Remove(entity);
            await _context.SaveChangesAsync();

            // silinen malzeme sonrası tarif besin değerini yeniden hesaplıyoruz
            await _nutritionService.SaveNutritionForRecipeAsync(recipeId);

            return true;
        }
    }
}
