namespace AkilliYemekTarifOneriSistemi.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // 🔥 API araması için İngilizce isim
        public string? EnglishName { get; set; }
    }
}
