using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AkilliYemekTarifOneriSistemi.Models
{
    
    
    
    
    public class Recipe
    {
        
        
        
        public int Id { get; set; }

        
        
        

        
        
        
        [StringLength(100, ErrorMessage = "Tarif Adı En Fazla 100 Karakter Olmalıdır...")]
        public string? Name { get; set; }

        
        
        
        [StringLength(500, ErrorMessage = "Açıklama En Fazla 500 Karakter İçermelidir...")]
        public string? Description { get; set; }

        
        
        
        [Range(1, 300, ErrorMessage = "Pişirme Süresi 1-300 Dakika Arasında Olmalıdır...")]
        public int CookingTime { get; set; }

        
        
        
        [Range(1, 20, ErrorMessage = "Servis Sayısı 1-20 Arasında olmalıdır...")]
        public int? Servings { get; set; }

        
        
        
        public string? DietType { get; set; }

        
        
        
        [StringLength(2000, ErrorMessage = "Hazırlanış en fazla 2000 karakter olmalıdır.")]
        public string? Instructions { get; set; }

        
        
        
        public string? ImagePath { get; set; }

        
        
        
        public NutritionFacts? NutritionFacts { get; set; }

        
        
        

        
        
        
        
        [Required(ErrorMessage = "Tarif başlığı zorunludur")]
        public string Title { get; set; } = string.Empty;

        
        
        
        public string? ShortDescription { get; set; }

        
        
        
        public string? ImageUrl { get; set; }

        
        
        
        public int? PreparationTimeMinutes { get; set; }

        
        
        
        public string? Difficulty { get; set; }

        
        
        
        public int? CategoryId { get; set; }

        
        
        
        public Category? Category { get; set; }

        public String? MealTags { get; set; }

        
        
        

        
        
        
        
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        
        
        
        public ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();

        
        
        
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = new List<FavoriteRecipe>();
    }
}
