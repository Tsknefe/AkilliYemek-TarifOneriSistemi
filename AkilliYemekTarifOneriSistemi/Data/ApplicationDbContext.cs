using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Data
{
    // burası uygulamanın ana veritabanı context i
    // identityDbContext den kalıtım alıyor çünkü kullanıcı yönetimi hazır geliyor
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // tabloları tek tek tanımlıyoruz
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<NutritionFacts> NutritionFacts { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserAllergy> UserAllergies { get; set; }

        // ilişkileri burda tanımlıyoruz çünkü ef core otomatik hepsini anlayamayabiliyor
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // identity sisteminin kendi tabloları da oluşsun diye base çağrısı lazım
            base.OnModelCreating(builder);

            // recipe ile recipeIngredient arasında bire çok ilişki var
            // bir tarifin birden fazla malzemesi olur
            builder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade); // tarif silinince malzemeleri de silinsin

            // ingredient ile recipeIngredient arasında yine bire çok ilişki var
            // bir malzeme bir sürü tarifte geçebilir ama burda navigation yok o yüzden WithMany() boş
            builder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany()
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // recipe ile nutritionFacts arasında bire bir ilişki var
            // bir tarifin tek besin bilgisi olur
            builder.Entity<NutritionFacts>()
                .HasOne(e => e.Recipe)
                .WithOne(f => f.NutritionFacts)
                .HasForeignKey<NutritionFacts>(e => e.RecipeId);
        }
    }
}
