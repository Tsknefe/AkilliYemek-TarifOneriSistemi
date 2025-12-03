namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Recipe ile Ingredient arasındaki N-N ilişkiyi temsil eden ara tablo (join entity).
    /// Her satır, belirli bir tarif için kullanılan tek bir malzemeyi ve miktarını ifade eder.
    /// Örn: "Mercimek Çorbası" tarifinde "Kırmızı Mercimek" 200 g.
    /// </summary>
    public class RecipeIngredient
    {
        /// <summary>
        /// İlişkinin tarif tarafındaki foreign key'i.
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// İlişkinin malzeme (ingredient) tarafındaki foreign key'i.
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Bu malzemenin tarifte kullanılan miktarı.
        /// Örn: 200, 1.5 gibi sayısal değerler.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Bu satır için kullanılan ölçü birimi.
        /// Örn: "g", "ml", "adet", "yemek kaşığı"
        /// Ingredient.DefaultUnit'ten farklı olabilir (tarife özel).
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Tarife özel notlar.
        /// Örn: "ince doğranmış", "kabukları soyulmuş"
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Navigation property - bu satırın bağlı olduğu tarif.
        /// EF Core bu alan sayesinde Recipe -> RecipeIngredients ilişkisini kurar.
        /// </summary>
        public Recipe Recipe { get; set; } = null!;

        /// <summary>
        /// Navigation property - bu satırın bağlı olduğu malzeme.
        /// EF Core bu alan sayesinde Ingredient -> RecipeIngredients ilişkisini kurar.
        /// </summary>
        public Ingredient Ingredient { get; set; } = null!;
    }
}



