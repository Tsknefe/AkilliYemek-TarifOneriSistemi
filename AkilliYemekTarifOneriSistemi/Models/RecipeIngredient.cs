namespace AkilliYemekTarifOneriSistemi.Models
{
    
    
    
    
    
    public class RecipeIngredient
    {
        
        public int Id { get; set; }

        
        
        
        public int RecipeId { get; set; }

        
        
        
        public int IngredientId { get; set; }

        

        
        
        
        
        public string Quantity { get; set; } = string.Empty;

        
        
        
        
        
        public double CalculatedGrams { get; set; }

        

        
        
        
        
        public decimal Amount { get; set; }

        
        
        
        
        public string? Unit { get; set; }

        
        
        
        
        public string? Note { get; set; }

        
        
        
        public Recipe Recipe { get; set; } = null!;

        
        
        
        public Ingredient Ingredient { get; set; } = null!;
    }
}
