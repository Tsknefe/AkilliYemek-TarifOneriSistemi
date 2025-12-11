using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    // bu servis tariflerle ilgili bütün temel işlemleri yapan yer
    // crud operasyonları burada dönüyor ve controller sadece burayı çağırıyor
    // besin değerleri veya malzeme kısmı burada değil onlar kendi servislerinde
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext _context;

        public RecipeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // tüm tarifleri listelediğimiz metod
        // arama varsa isim açıklama veya diyet tipine göre filtreleme yapıyoruz
        // include ler ile besin değerleri ve malzemeleri de birlikte çekiyoruz çünkü frontend bunları gösteriyor
        public async Task<List<Recipe>> GetAllAsync(string? search = null)
        {
            var query = _context.Recipes
                .Include(r => r.NutritionFacts)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .AsQueryable();

            // arama kutusu boş değilse filtre uyguluyoruz
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    r.Name.Contains(search) ||
                    r.Description.Contains(search) ||
                    r.DietType.Contains(search));
            }

            return await query.ToListAsync();
        }

        // tek bir tarifi id ile getirme
        // detay ekranı bunu kullanıyor
        // include lerle ilişkili tüm veriler birlikte geliyor
        public async Task<Recipe?> GetByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.NutritionFacts)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // yeni tarif ekleme
        // burada basitçe kaydedip geri dönüyoruz başka işlem yok
        public async Task<Recipe> CreateAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        // tarif güncelleme işlemi
        // önce tarif var mı diye buluyoruz yoksa null dönüyoruz
        // sonra alanları tek tek güncelliyoruz
        public async Task<Recipe?> UpdateAsync(Recipe recipe)
        {
            var existing = await _context.Recipes.FindAsync(recipe.Id);
            if (existing == null)
                return null;

            existing.Name = recipe.Name;
            existing.Description = recipe.Description;
            existing.CookingTime = recipe.CookingTime;
            existing.Servings = recipe.Servings;
            existing.DietType = recipe.DietType;

            await _context.SaveChangesAsync();
            return existing;
        }

        // tarif silme işlemi
        // tarif varsa siliyoruz yoksa false
        public async Task<bool> DeleteAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return false;

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
