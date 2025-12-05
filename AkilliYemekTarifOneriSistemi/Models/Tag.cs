using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Tariflere serbest etiket (tag) atamak için kullanılır.
    /// Örn: "Vegan", "Glutensiz", "Çocuklar için"
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Veritabanı için birincil anahtar (Primary Key).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Etiket adı.
        /// Örn: "Vegan", "Glutensiz"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bu etikete sahip tarifler ile arasındaki N-N ilişkileri tutan ara tablo kayıtları.
        /// </summary>
        public ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();
    }
}



