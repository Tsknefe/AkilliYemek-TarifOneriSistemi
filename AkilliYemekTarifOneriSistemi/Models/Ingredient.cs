using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Bir tarifte kullanılabilecek tek bir malzemeyi (ingredient) temsil eder.
    /// Örn: Domates, Zeytinyağı, Tavuk Göğsü vb.
    /// </summary>
    public class Ingredient
    {
        /// <summary>
        /// Veritabanı için birincil anahtar (Primary Key).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Kullanıcının gördüğü Türkçe isim.
        /// Örn: "Domates", "Zeytinyağı"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// API aramaları için İngilizce isim.
        /// Örn: "Tomato", "Olive Oil"
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// Malzeme genelde hangi birim ile ölçülür?
        /// Örn: "g", "ml", "adet", "yemek kaşığı"
        /// </summary>
        public string? DefaultUnit { get; set; }

        /// <summary>
        /// İsteğe bağlı açıklama alanı.
        /// Örn: "Organik domates", "Soğuk sıkım zeytinyağı"
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Bu malzemenin kullanıldığı tarif–malzeme ilişkileri.
        /// Bir Ingredient, birden fazla RecipeIngredient kaydında yer alabilir.
        /// </summary>
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}
