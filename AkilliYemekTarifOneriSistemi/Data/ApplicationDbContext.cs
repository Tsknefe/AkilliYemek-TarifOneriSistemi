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
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // -----------------------------
        // SENİN ESKİ TABLOLARIN
        // -----------------------------
        public DbSet<NutritionFacts> NutritionFacts { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<UserAllergy> UserAllergies { get; set; } = null!;

        // -----------------------------
        // GÜNCEL YEMEK SİSTEMİ TABLOLARI
        // -----------------------------

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
        /// Model oluşturulurken (migration sırasında) ilişki ve tablo ayarlarını yapar.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Identity tabloları vs için temel konfigürasyon
            base.OnModelCreating(builder);

            // -----------------------------
            // RecipeIngredient: Recipe ve Ingredient arasındaki N-N ilişki
            // -----------------------------
            builder.Entity<RecipeIngredient>(entity =>
            {
                // PK artık Id
                entity.HasKey(ri => ri.Id);

                // Aynı tarif + aynı malzeme bir kez olsun istiyorsan unique index:
                entity.HasIndex(ri => new { ri.RecipeId, ri.IngredientId })
                      .IsUnique();

                entity.HasOne(ri => ri.Recipe)
                      .WithMany(r => r.RecipeIngredients)
                      .HasForeignKey(ri => ri.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ri => ri.Ingredient)
                      .WithMany(i => i.RecipeIngredients)
                      .HasForeignKey(ri => ri.IngredientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ri => ri.Amount)
                      .HasPrecision(10, 2);
            });


            // -----------------------------
            // RecipeTag: Recipe ve Tag arasındaki N-N ilişki
            // -----------------------------
            builder.Entity<RecipeTag>(entity =>
            {
                entity.HasKey(rt => new { rt.RecipeId, rt.TagId });

                entity.HasOne(rt => rt.Recipe)
                      .WithMany(r => r.RecipeTags)
                      .HasForeignKey(rt => rt.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rt => rt.Tag)
                      .WithMany(t => t.RecipeTags)
                      .HasForeignKey(rt => rt.TagId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -----------------------------
            // FavoriteRecipe: Kullanıcı ile Recipe arasındaki N-N ilişki
            // -----------------------------
            builder.Entity<FavoriteRecipe>(entity =>
            {
                // Composite Primary Key: Aynı kullanıcı aynı tarifi sadece bir kez favorileyebilsin.
                entity.HasKey(fr => new { fr.UserId, fr.RecipeId });

                // FavoriteRecipe -> User
                entity.HasOne(fr => fr.User)
                      .WithMany()
                      .HasForeignKey(fr => fr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // FavoriteRecipe -> Recipe
                entity.HasOne(fr => fr.Recipe)
                      .WithMany(r => r.FavoriteRecipes)
                      .HasForeignKey(fr => fr.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -----------------------------
            // WeeklyPlan: Kullanıcıya ait haftalık plan
            // -----------------------------
            builder.Entity<WeeklyPlan>(entity =>
            {
                entity.HasOne(wp => wp.User)
                      .WithMany()
                      .HasForeignKey(wp => wp.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -----------------------------
            // WeeklyPlanItem: Haftalık plan içindeki tek tek öğün satırları
            // -----------------------------
            builder.Entity<WeeklyPlanItem>(entity =>
            {
                entity.HasOne(item => item.WeeklyPlan)
                      .WithMany(wp => wp.Items)
                      .HasForeignKey(item => item.WeeklyPlanId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(item => item.Recipe)
                      .WithMany()
                      .HasForeignKey(item => item.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Aynı planda aynı gün + öğün kombinasyonu bir kez tanımlansın:
                entity.HasIndex(item => new { item.WeeklyPlanId, item.DayOfWeek, item.MealType })
                      .IsUnique();
            });

            // -----------------------------
            // SENİN ÖNCEKİ İLİŞKİLERİN
            // -----------------------------

            // Recipe <-> NutritionFacts : bire bir ilişki
            builder.Entity<NutritionFacts>()
                .HasOne(e => e.Recipe)
                .WithOne(r => r.NutritionFacts)
                .HasForeignKey<NutritionFacts>(e => e.RecipeId);

            // Eğer UserProfile / UserAllergy için özel ilişki konfigürasyonların varsa
            // (eski context’te vardıysa) onları da buraya ekleyebilirsin.
        }
    }
}
