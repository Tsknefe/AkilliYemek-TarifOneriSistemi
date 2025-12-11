using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    // bu service tamamen kullanıcının alerjilerini yönetmek için yazıldı
    // kullanıcı bir malzemeye alerjisi olduğunu işaretlediğinde ya da kaldırdığında
    // bu sınıf devreye giriyor
    public class AllergyService : IAllergyService
    {
        private readonly ApplicationDbContext _context;

        public AllergyService(ApplicationDbContext context)
        {
            _context = context;
        }

        // bir kullanıcının hangi malzemelere alerjisi olduğunu sadece id olarak getiriyoruz
        // Recommendation Service bu id listesine göre tarifleri eleyebiliyor
        public async Task<List<int>> GetUserAllergyIngredientIdsAsync(string userId)
        {
            return await _context.UserAllergies
                .Where(x => x.UserId == userId)
                .Select(x => x.IngredientId)
                .ToListAsync();
        }

        // burada alerji kayıtlarını Ingredient navigation property ile birlikte çekiyoruz
        // böylece sayfalarda "ingredient.Name" olarak rahatça yazabiliyoruz
        public async Task<List<UserAllergy>> GetUserAllergiesAsync(string userId)
        {
            return await _context.UserAllergies
                .Include(x => x.Ingredient)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        // kullanıcı aynı alerjiyi ikinci kez eklemesin diye önce kontrol ediyoruz
        // yoksa duplicate kayıt olur ve UI tarafında da sorun çıkar
        public async Task AddAllergyAsync(string userId, int ingredientId)
        {
            bool exists = await _context.UserAllergies
                .AnyAsync(x => x.UserId == userId && x.IngredientId == ingredientId);

            if (exists)
                return;

            _context.UserAllergies.Add(new UserAllergy
            {
                UserId = userId,
                IngredientId = ingredientId
            });

            await _context.SaveChangesAsync();
        }

        // alerji kaydı varsa kaldırıyoruz
        // yoksa hiçbir şey yapmadan çıkıyoruz çünkü hata üretmeye gerek yok
        public async Task RemoveAllergyAsync(string userId, int ingredientId)
        {
            var entity = await _context.UserAllergies
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IngredientId == ingredientId);

            if (entity == null)
                return;

            _context.UserAllergies.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
