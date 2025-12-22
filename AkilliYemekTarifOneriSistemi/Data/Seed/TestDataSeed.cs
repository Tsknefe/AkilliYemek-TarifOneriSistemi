using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Data.Seed
{
    public static class TestDataSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            
            
            
            if (!context.Ingredients.Any())
            {
                var ingredients = new List<Ingredient>
                {
                    new() { Name = "Domates", EnglishName = "Tomato", DefaultUnit = "g" },
                    new() { Name = "Soðan", EnglishName = "Onion", DefaultUnit = "g" },
                    new() { Name = "Tavuk Göðsü", EnglishName = "Chicken Breast", DefaultUnit = "g" },
                    new() { Name = "Zeytinyaðý", EnglishName = "Olive Oil", DefaultUnit = "ml" },
                    new() { Name = "Sarýmsak", EnglishName = "Garlic", DefaultUnit = "adet" },
                    new() { Name = "Pirinç", EnglishName = "Rice", DefaultUnit = "g" },
                    new() { Name = "Makarna", EnglishName = "Pasta", DefaultUnit = "g" }
                };

                context.Ingredients.AddRange(ingredients);
                await context.SaveChangesAsync();
            }

            
            
            
            if (!context.Recipes.Any())
            {
                var recipe = new Recipe
                {
                    Title = "Tavuklu Sebze Sote",
                    Description = "Protein aðýrlýklý, pratik bir akþam yemeði",
                    CookingTime = 25,
                    Servings = 2,
                    DietType = "standard",
                    ImageUrl = "https://via.placeholder.com/600x400"
                };

                context.Recipes.Add(recipe);
                await context.SaveChangesAsync();

                
                
                
                context.NutritionFacts.Add(new NutritionFacts
                {
                    RecipeId = recipe.Id,
                    Calories = 520,
                    Protein = 45,
                    Fat = 18,
                    Carbs = 30,
                    Sugar = 6,
                    Fiber = 4
                });

                await context.SaveChangesAsync();

                
                
                
                var domates = await context.Ingredients.FirstAsync(x => x.Name == "Domates");
                var tavuk = await context.Ingredients.FirstAsync(x => x.Name == "Tavuk Göðsü");
                var zeytinyagi = await context.Ingredients.FirstAsync(x => x.Name == "Zeytinyaðý");

                context.RecipeIngredients.AddRange(
                    new RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = domates.Id,
                        Quantity = Convert.ToString(150),
                        Unit = "g",
                        CalculatedGrams = 150
                    },
                    new RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = tavuk.Id,
                        Quantity = Convert.ToString(300),
                        Unit = "g",
                        CalculatedGrams = 300
                    },
                    new RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = zeytinyagi.Id,
                        Quantity = Convert.ToString(15),
                        Unit = "ml",
                        CalculatedGrams = 15
                    }
                );

                await context.SaveChangesAsync();
            }
        }
        private static async Task<Ingredient> GetOrCreateIngredientAsync(
    ApplicationDbContext context,
    string name,
    string? englishName = null,
    string? defaultUnit = null,
    string? description = null)
        {
            var normalized = name.Trim();

            var existing = await context.Ingredients
                .FirstOrDefaultAsync(x => x.Name.Trim() == normalized);

            if (existing != null) return existing;

            var entity = new Ingredient
            {
                Name = normalized,
                EnglishName = englishName,
                DefaultUnit = defaultUnit,
                Description = description
            };

            context.Ingredients.Add(entity);
            await context.SaveChangesAsync(); 
            return entity;
        }

    }
}
