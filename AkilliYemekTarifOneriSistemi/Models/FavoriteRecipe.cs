using System;
using Microsoft.AspNetCore.Identity;

namespace AkilliYemekTarifOneriSistemi.Models
{
    
    
    
    
    
    public class FavoriteRecipe
    {
        
        
        
        
        public string UserId { get; set; } = string.Empty;

        
        
        
        public int RecipeId { get; set; }

        
        
        
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        
        
        
        public IdentityUser User { get; set; } = null!;

        
        
        
        public Recipe Recipe { get; set; } = null!;
    }
}


