using Microsoft.EntityFrameworkCore;
using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    // burası tamamen ingredient yani malzeme işlemlerinin yapıldığı service
    // controller lar burayı çağırıyor çünkü tüm veri işlemleri servis katmanında duruyor
    // yani crud işlemlerini tek merkezde topladığımız yer burası

    public class IngredientService : IIngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        // tüm malzemeleri listelemek için
        // search parametresi istersek filtreleme yapabilmek için var
        // ama şu an direkt hepsini dönüyoruz
        public async Task<List<Ingredient>> GetAllAsync(string? search = null)
        {
            return await _context.Ingredients.ToListAsync();
        }

        // tek bir malzemeyi id ye göre getiriyoruz
        // edit ve delete işlemleri bunu kullanıyor
        public async Task<Ingredient?> GetByIdAsync(int id)
        {
            return await _context.Ingredients.FindAsync(id);
        }

        // yeni malzeme ekleme işlemi
        // burada doğrudan ekleyip kaydediyoruz
        public async Task<Ingredient> CreateAsync(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        // var olan bir malzemeyi güncelleme işlemi
        // update ile tracklenen entity güncelleniyor
        public async Task UpdateAsync(Ingredient ingredient)
        {
            _context.Ingredients.Update(ingredient);
            await _context.SaveChangesAsync();
        }

        // malzeme silme işlemi
        // önce id ile malzeme var mı diye bakıyoruz yoksa çıkıyoruz
        public async Task DeleteAsync(int id)
        {
            var ingredient = await GetByIdAsync(id);
            if (ingredient is null) return;

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
        }
    }
}
