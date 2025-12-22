using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    public class AllergyService : IAllergyService
    {
        private readonly ApplicationDbContext _context;

        public AllergyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetUserAllergyIngredientIdsAsync(string userId)
        {
            return await _context.UserAllergies
                .Where(x => x.UserId == userId)
                .Select(x => x.IngredientId)
                .ToListAsync();
        }

        public async Task<List<UserAllergy>> GetUserAllergiesAsync(string userId)
        {
            return await _context.UserAllergies
                .Include(x => x.Ingredient)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

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
