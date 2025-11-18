using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<NutritionFacts> NutritionFacts { get; set;}
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get;set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //veritabanı için gerekli ilişkileri tanımlıyoruz biliyorsunuz zaten yinede bahsettim :) 

            //1-n
            builder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RecipeIngredient>()
                 .HasOne(ri => ri.Ingredient)
                 .WithMany()
                 .HasForeignKey(ri => ri.IngredientId)
                 .OnDelete(DeleteBehavior.Cascade);

            //1-1
            builder.Entity<NutritionFacts>()
                .HasOne(e => e.Recipe)
                .WithOne(f => f.NutritionFacts)
                .HasForeignKey<NutritionFacts>(e => e.RecipeId);
        }
    }
}
