namespace AkilliYemekTarifOneriSistemi.Models
{
    
    
    
    
    public class ShoppingListItem
    {
        
        
        
        public string IngredientName { get; set; } = string.Empty;

        
        
        
        public decimal TotalAmountInGrams { get; set; }

        
        
        
        public string DisplayUnit { get; set; } = "g";

        
        
        
        
        public decimal DisplayAmount { get; set; }

        
        
        
        public List<string> UsedInRecipes { get; set; } = new List<string>();
    }
}

