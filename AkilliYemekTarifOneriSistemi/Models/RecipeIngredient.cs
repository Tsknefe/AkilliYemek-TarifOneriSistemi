namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Recipe ile Ingredient arasındaki ilişkiyi temsil eden ara tablo (join entity).
    /// Her satır, belirli bir tarif için kullanılan tek bir malzemeyi ve miktarını ifade eder.
    /// Örn: "Mercimek Çorbası" tarifinde "Kırmızı Mercimek" 200 g.
    /// </summary>
    public class RecipeIngredient
    {
        // Eski koddaki Id alanı (PK)
        public int Id { get; set; }

        /// <summary>
        /// İlişkinin tarif tarafındaki foreign key'i.
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// İlişkinin malzeme (ingredient) tarafındaki foreign key'i.
        /// </summary>
        public int IngredientId { get; set; }

        // ----------------- ESKİ ALANLAR (UYUMLULUK İÇİN) -----------------

        /// <summary>
        /// Kullanıcının yazdığı ham miktar (metin).
        /// Örn: "2 su bardağı", "1 yemek kaşığı".
        /// </summary>
        public string Quantity { get; set; } = string.Empty;

        /// <summary>
        /// Quantity'nin gram cinsinden hesaplanmış hali.
        /// Örn: 1 su bardağı pirinç ≈ 160 g.
        /// Öneri ve alışveriş listesi hesaplarında kullanılıyor.
        /// </summary>
        public double CalculatedGrams { get; set; }

        // ----------------- YENİ ALANLAR -----------------

        /// <summary>
        /// Sayısal miktar. Örn: 200, 1.5
        /// (İstersen ileride Quantity yerine bunu kullanırsın.)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Bu satır için kullanılan ölçü birimi.
        /// Örn: "g", "ml", "adet", "yemek kaşığı"
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Tarife özel notlar.
        /// Örn: "ince doğranmış", "kabukları soyulmuş"
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Navigation property - tarif.
        /// </summary>
        public Recipe Recipe { get; set; } = null!;

        /// <summary>
        /// Navigation property - malzeme.
        /// </summary>
        public Ingredient Ingredient { get; set; } = null!;
    }
}
