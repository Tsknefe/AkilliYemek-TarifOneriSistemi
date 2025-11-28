namespace AkilliYemekTarifOneriSistemi.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class RecipeIngredient
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        public int IngredientId { get; set; }
        public Ingredient? Ingredient { get; set; }

        // Kullanıcının yazdığı miktar
        public string Quantity { get; set; }

        // Kullanıcı birimi (kg, g, adet, yemek kaşığı...)
        public string Unit { get; set; } = string.Empty;

        // 🔥 GRAM cinsinden hesaplanan gerçek miktar
        public double CalculatedGrams { get; set; }
    }
}
