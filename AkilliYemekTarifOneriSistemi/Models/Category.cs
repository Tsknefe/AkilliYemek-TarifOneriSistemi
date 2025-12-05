using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Tariflerin ait olduğu kategori bilgisini tutar.
    /// Örn: "Çorba", "Ana Yemek", "Tatlı"
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Veritabanı için birincil anahtar (Primary Key).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Kategori adı.
        /// Örn: "Çorba", "Salata"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// İsteğe bağlı açıklama alanı.
        /// Örn: "Sıcak çorbalar", "Soğuk başlangıçlar"
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Bu kategoriye ait tarifler.
        /// Bir Category, birçok Recipe içerebilir (1 - N ilişki).
        /// </summary>
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}



