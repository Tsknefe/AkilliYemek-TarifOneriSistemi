using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Data
{
    /// <summary>
    /// Uygulamanın ana veritabanı bağlamı (EF Core DbContext).
    /// Hem Identity tablolarını, hem de proje domain tablolarını (Recipe, Ingredient vb.) içerir.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Yemek tarifleri (Recipe) için tabloyu temsil eder.
        /// </summary>
        public DbSet<Recipe> Recipes { get; set; } = null!;

        /// <summary>
        /// Malzemeler (Ingredient) için tabloyu temsil eder.
        /// </summary>
        public DbSet<Ingredient> Ingredients { get; set; } = null!;

        /// <summary>
        /// Tarif – malzeme ilişkisini ve miktar bilgilerini tutan ara tabloyu temsil eder.
        /// </summary>
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = null!;

        /// <summary>
        /// Tarif kategorileri (Category) için tabloyu temsil eder.
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null!;

        /// <summary>
        /// Tarif etiketleri (Tag) için tabloyu temsil eder.
        /// </summary>
        public DbSet<Tag> Tags { get; set; } = null!;

        /// <summary>
        /// Recipe ile Tag arasındaki N-N ilişkiyi tutan ara tabloyu temsil eder.
        /// </summary>
        public DbSet<RecipeTag> RecipeTags { get; set; } = null!;

        /// <summary>
        /// Kullanıcıların favori olarak işaretlediği tarifleri tutan ara tabloyu temsil eder.
        /// </summary>
        public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; } = null!;

        /// <summary>
        /// Kullanıcıların oluşturduğu haftalık planları (WeeklyPlan) temsil eder.
        /// </summary>
        public DbSet<WeeklyPlan> WeeklyPlans { get; set; } = null!;

        /// <summary>
        /// Haftalık plan içindeki tek tek öğün/hücre kayıtlarını (WeeklyPlanItem) temsil eder.
        /// </summary>
        public DbSet<WeeklyPlanItem> WeeklyPlanItems { get; set; } = null!;

        /// <summary>
        /// Model oluşturulurken (migration oluşturma sırasında) ilişki ve tablo ayarlarını yapar.
        /// Burada RecipeIngredient için composite key ve ilişkiler tanımlanır.
        /// </summary>
        /// <param name="builder">ModelBuilder nesnesi</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // RecipeIngredient: Recipe ve Ingredient arasındaki N-N ilişki için ara tablo
            builder.Entity<RecipeIngredient>(entity =>
            {
                // Composite Primary Key: Aynı tarif için aynı malzeme bir kere tanımlansın.
                entity.HasKey(ri => new { ri.RecipeId, ri.IngredientId });

                // RecipeIngredient -> Recipe (N-1) ilişkisi
                entity.HasOne(ri => ri.Recipe)
                      .WithMany(r => r.RecipeIngredients)
                      .HasForeignKey(ri => ri.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);

                // RecipeIngredient -> Ingredient (N-1) ilişkisi
                entity.HasOne(ri => ri.Ingredient)
                      .WithMany(i => i.RecipeIngredients)
                      .HasForeignKey(ri => ri.IngredientId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Amount alanı için hassasiyet (Precision) ayarı.
                // Örn: 9999.99 gibi değerler tutulabilir.
                entity.Property(ri => ri.Amount)
                      .HasPrecision(10, 2);
            });

            // RecipeTag: Recipe ve Tag arasındaki N-N ilişki için ara tablo
            builder.Entity<RecipeTag>(entity =>
            {
                // Composite Primary Key: Aynı tarif için aynı etiket bir kere tanımlansın.
                entity.HasKey(rt => new { rt.RecipeId, rt.TagId });

                // RecipeTag -> Recipe (N-1) ilişkisi
                entity.HasOne(rt => rt.Recipe)
                      .WithMany(r => r.RecipeTags)
                      .HasForeignKey(rt => rt.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);

                // RecipeTag -> Tag (N-1) ilişkisi
                entity.HasOne(rt => rt.Tag)
                      .WithMany(t => t.RecipeTags)
                      .HasForeignKey(rt => rt.TagId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // FavoriteRecipe: Kullanıcı ile Recipe arasındaki N-N ilişki için ara tablo
            builder.Entity<FavoriteRecipe>(entity =>
            {
                // Composite Primary Key: Aynı kullanıcı aynı tarifi sadece bir kez favorileyebilsin.
                entity.HasKey(fr => new { fr.UserId, fr.RecipeId });

                // FavoriteRecipe -> User (N-1) ilişkisi
                // Identity tarafındaki AspNetUsers tablosuna bağlanır.
                entity.HasOne(fr => fr.User)
                      .WithMany() // Şu an IdentityUser tarafında navigation tanımlamıyoruz.
                      .HasForeignKey(fr => fr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // FavoriteRecipe -> Recipe (N-1) ilişkisi
                entity.HasOne(fr => fr.Recipe)
                      .WithMany(r => r.FavoriteRecipes)
                      .HasForeignKey(fr => fr.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // WeeklyPlan: Kullanıcıya ait haftalık plan
            builder.Entity<WeeklyPlan>(entity =>
            {
                // WeeklyPlan -> User (N-1) ilişkisi
                entity.HasOne(wp => wp.User)
                      .WithMany() // Şimdilik IdentityUser tarafında navigation yok.
                      .HasForeignKey(wp => wp.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // StartDate için sadece tarih kısmı önemli ise, isteğe bağlı olarak precision ayarı yapılabilir.
            });

            // WeeklyPlanItem: Haftalık plan içindeki tek tek öğün/hücre satırları
            builder.Entity<WeeklyPlanItem>(entity =>
            {
                // WeeklyPlanItem -> WeeklyPlan (N-1) ilişkisi
                entity.HasOne(item => item.WeeklyPlan)
                      .WithMany(wp => wp.Items)
                      .HasForeignKey(item => item.WeeklyPlanId)
                      .OnDelete(DeleteBehavior.Cascade);

                // WeeklyPlanItem -> Recipe (N-1) ilişkisi
                entity.HasOne(item => item.Recipe)
                      .WithMany() // İstenirse Recipe tarafında WeeklyPlanItems koleksiyonu eklenebilir.
                      .HasForeignKey(item => item.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Bir haftalık planda, aynı gün + öğün kombinasyonunun bir kez tanımlanması mantıklı olabilir.
                // Bunun için composite unique index tanımlanabilir:
                entity.HasIndex(item => new { item.WeeklyPlanId, item.DayOfWeek, item.MealType })
                      .IsUnique();
            });
        }
    }
}
