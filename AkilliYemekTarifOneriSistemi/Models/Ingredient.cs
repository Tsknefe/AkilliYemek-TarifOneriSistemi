namespace AkilliYemekTarifOneriSistemi.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        // Kullanıcının gördüğü Türkçe isim
        public string Name { get; set; } = string.Empty;

        // API araması için İngilizce isim
        public string? EnglishName { get; set; }
    }
}
