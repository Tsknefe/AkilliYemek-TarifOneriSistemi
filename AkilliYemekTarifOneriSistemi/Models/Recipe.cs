using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        // -----------------------------
        // Eski modelden gelen alanlar
        // -----------------------------

        /// <summary>
        /// Eski admin tarafında kullanılan tarif adı.
        /// </summary>
        [StringLength(100, ErrorMessage = "Tarif Adı En Fazla 100 Karakter Olmalıdır...")]
        public string? Name { get; set; }

        /// <summary>
        /// Detay açıklama (metin).
        /// </summary>
        [StringLength(500, ErrorMessage = "Açıklama En Fazla 500 Karakter İçermelidir...")]
        public string? Description { get; set; }

        /// <summary>
        /// Pişirme süresi (dakika).
        /// </summary>
        [Range(1, 300, ErrorMessage = "Pişirme Süresi 1-300 Dakika Arasında Olmalıdır...")]
        public int CookingTime { get; set; }

        /// <summary>
        /// Kaç kişilik olduğu.
        /// </summary>
        [Range(1, 20, ErrorMessage = "Servis Sayısı 1-20 Arasında olmalıdır...")]
        public int? Servings { get; set; }

        /// <summary>
        /// Diyet tipi (vegan, vejetaryen vb.).
        /// </summary>
        public string? DietType { get; set; }

        /// <summary>
        /// Hazırlanış metni (adım adım).
        /// </summary>
        [StringLength(2000, ErrorMessage = "Hazırlanış en fazla 2000 karakter olmalıdır.")]
        public string? Instructions { get; set; }

        /// <summary>
        /// Eski tarafta kullanılan görsel yolu (dosya path).
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Besin değerleri ile bire bir ilişki.
        /// </summary>
        public NutritionFacts? NutritionFacts { get; set; }

        // -----------------------------
        // Yeni PR tarafındaki alanlar
        // -----------------------------

        /// <summary>
        /// Yeni kart/grid arayüzünde görünen başlık.
        /// Örn: "Fırında Baharatlı Tavuk"
        /// </summary>
        [Required(ErrorMessage = "Tarif başlığı zorunludur")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Kartta gösterilecek kısa açıklama.
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Kart/detay sayfasında kullanılacak görsel URL'i.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Tahmini hazırlama süresi (dakika cinsinden).
        /// </summary>
        public int? PreparationTimeMinutes { get; set; }

        /// <summary>
        /// Zorluk seviyesi: Kolay / Orta / Zor.
        /// </summary>
        public string? Difficulty { get; set; }

        /// <summary>
        /// Tarifin ait olduğu kategori (Çorba, Ana Yemek, vb.).
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Navigation property - Bu tarifin bağlı olduğu kategori.
        /// </summary>
        public Category? Category { get; set; }

        // -----------------------------
        // İlişkiler
        // -----------------------------

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
