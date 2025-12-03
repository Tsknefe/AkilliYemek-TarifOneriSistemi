using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Uygulamadaki bir yemek tarifini temsil eder.
    /// Örn: "Fırında Tavuk", "Mercimek Çorbası"
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Veritabanı için birincil anahtar (Primary Key).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tarifin görünen başlığı.
        /// Örn: "Fırında Baharatlı Tavuk"
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Tarifin kısa açıklaması.
        /// Örn: "Akşam yemekleri için pratik ve lezzetli bir tavuk tarifi."
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Tarifin hazırlanış (adım adım) açıklaması.
        /// Metin uzun olabilir; bu yüzden ayrı bir alan olarak tutulur.
        /// </summary>
        public string? Instructions { get; set; }

        /// <summary>
        /// Tarif görselinin URL bilgisi.
        /// Frontend bu url üzerinden resmi gösterebilir.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Tarifin tahmini hazırlama süresi (dakika cinsinden).
        /// Örn: 30, 45, 60
        /// </summary>
        public int? PreparationTimeMinutes { get; set; }

        /// <summary>
        /// Tarifin zorluk seviyesi.
        /// Örn: "Kolay", "Orta", "Zor"
        /// Basitlik için string tutuluyor; istenirse enum'a çevrilebilir.
        /// </summary>
        public string? Difficulty { get; set; }

        /// <summary>
        /// Tarifin ait olduğu kategori (Çorba, Ana Yemek, vb.).
        /// Bir Recipe, isteğe bağlı olarak bir Category'e bağlanabilir.
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Navigation property - Bu tarifin bağlı olduğu kategori.
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// Bu tarifte kullanılan malzemeleri temsil eden ara tablo kayıtları.
        /// Bir Recipe, birçok RecipeIngredient satırına sahip olabilir.
        /// </summary>
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        /// <summary>
        /// Bu tarifin sahip olduğu etiketler (Tag) ile arasındaki N-N ilişkileri tutan ara tablo kayıtları.
        /// </summary>
        public ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();

        /// <summary>
        /// Bu tarifi favorilerine ekleyen kullanıcılarla arasındaki N-N ilişkiyi temsil eden ara tablo kayıtları.
        /// </summary>
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = new List<FavoriteRecipe>();
    }
}
