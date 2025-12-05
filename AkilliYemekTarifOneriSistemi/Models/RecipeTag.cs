namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Recipe ile Tag arasındaki N-N ilişkiyi temsil eden ara tablo (join entity).
    /// Her satır, bir tarifin bir etikete sahip olduğunu gösterir.
    /// Örn: "Mercimek Çorbası" tarifinin "Vegan" etiketi olması gibi.
    /// </summary>
    public class RecipeTag
    {
        /// <summary>
        /// İlişkinin tarif tarafındaki foreign key'i.
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// İlişkinin etiket (tag) tarafındaki foreign key'i.
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// Navigation property - bu satırın bağlı olduğu tarif.
        /// </summary>
        public Recipe Recipe { get; set; } = null!;

        /// <summary>
        /// Navigation property - bu satırın bağlı olduğu etiket.
        /// </summary>
        public Tag Tag { get; set; } = null!;
    }
}



