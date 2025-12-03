namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Alışveriş listesindeki tek bir malzeme satırını temsil eder.
    /// Birim dönüştürme ve birleştirme işlemlerinden sonra oluşur.
    /// </summary>
    public class ShoppingListItem
    {
        /// <summary>
        /// Malzemenin adı.
        /// </summary>
        public string IngredientName { get; set; } = string.Empty;

        /// <summary>
        /// Toplam miktar (gram cinsinden, birim dönüştürme sonrası).
        /// </summary>
        public decimal TotalAmountInGrams { get; set; }

        /// <summary>
        /// Görüntüleme için kullanılacak birim (g, kg, ml, adet vb.).
        /// </summary>
        public string DisplayUnit { get; set; } = "g";

        /// <summary>
        /// Görüntüleme için kullanılacak miktar (birim dönüştürme sonrası).
        /// Örn: 1500g yerine 1.5kg gösterilir.
        /// </summary>
        public decimal DisplayAmount { get; set; }

        /// <summary>
        /// Bu malzemenin hangi tariflerde kullanıldığının listesi (bilgilendirme amaçlı).
        /// </summary>
        public List<string> UsedInRecipes { get; set; } = new List<string>();
    }
}

