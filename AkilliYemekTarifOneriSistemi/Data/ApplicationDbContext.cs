using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Data
{
    
    
    
    
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        
        
        public DbSet<NutritionFacts> NutritionFacts { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<UserAllergy> UserAllergies { get; set; } = null!;

        
        
        

        
        
        
        public DbSet<Recipe> Recipes { get; set; } = null!;

        
        
        
        public DbSet<Ingredient> Ingredients { get; set; } = null!;

        
        
        
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = null!;

        
        
        
        public DbSet<Category> Categories { get; set; } = null!;

        
        
        
        public DbSet<Tag> Tags { get; set; } = null!;

        
        
        
        public DbSet<RecipeTag> RecipeTags { get; set; } = null!;

        
        
        
        public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; } = null!;

        
        
        
        public DbSet<WeeklyPlan> WeeklyPlans { get; set; } = null!;

        
        
        
        public DbSet<WeeklyPlanItem> WeeklyPlanItems { get; set; } = null!;

        
        
        
        public DbSet<SupportMessage> SupportMessages => Set<SupportMessage>();
        public DbSet<SupportThread> SupportThreads { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

            
            
            
            builder.Entity<RecipeIngredient>(entity =>
            {
                
                entity.HasKey(ri => ri.Id);

                
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

            
            
            
            builder.Entity<FavoriteRecipe>(entity =>
            {
                
                entity.HasKey(fr => new { fr.UserId, fr.RecipeId });

                
                entity.HasOne(fr => fr.User)
                      .WithMany()
                      .HasForeignKey(fr => fr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                
                entity.HasOne(fr => fr.Recipe)
                      .WithMany(r => r.FavoriteRecipes)
                      .HasForeignKey(fr => fr.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            
            
            
            builder.Entity<WeeklyPlan>(entity =>
            {
                entity.HasOne(wp => wp.User)
                      .WithMany()
                      .HasForeignKey(wp => wp.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            
            
            
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

                
                entity.HasIndex(item => new { item.WeeklyPlanId, item.DayOfWeek, item.MealType })
                      .IsUnique();
            });

            
            
            

            
            builder.Entity<NutritionFacts>()
                .HasOne(e => e.Recipe)
                .WithOne(r => r.NutritionFacts)
                .HasForeignKey<NutritionFacts>(e => e.RecipeId);

            

            
            builder.Entity<SupportThread>()
              .HasMany(t => t.Messages)
              .WithOne(m => m.Thread!)
              .HasForeignKey(m => m.ThreadId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
