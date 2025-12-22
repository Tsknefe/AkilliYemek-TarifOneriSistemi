using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Models
{
    
    
    
    
    public class Ingredient
    {
        
        
        
        public int Id { get; set; }

        
        
        
        
        public string Name { get; set; } = string.Empty;

        
        
        
        
        public string? EnglishName { get; set; }

        
        
        
        
        public string? DefaultUnit { get; set; }

        
        
        
        
        public string? Description { get; set; }

        
        
        
        
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}
