using Microsoft.AspNetCore.Identity;

namespace AkilliYemekTarifOneriSistemi.Models
{
    // kullanıcıya ait ekstra profil bilgilerini bu sınıfta tutuyoruz
    // identity nin kendi user tablosu yeterli olmadığı için böyle ayrı bir tablo oluşturduk
    public class UserProfile
    {
        // profil tablosunun kendi id si
        public int Id { get; set; }

        // identityUser ın id sini tutuyoruz böylece iki tablo birbirine bağlanıyor
        public string UserId { get; set; } = null!;

        // identityUser ile ilişki için navigation property
        public IdentityUser User { get; set; } = null!;

        // kullanıcının yaşı boyu kilosu gibi fiziksel değerler
        public int Age { get; set; }
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }

        // cinsiyet bilgisi
        public string Gender { get; set; } = "Male";

        // günlük aktivite seviyesi
        // bu bilgi ileride günlük kalori hesaplamalarında kullanılabiliyor
        public string ActivityLevel { get; set; } = "sedentary";

        // kullanıcının kilo hedefi
        // zayıflama koruma kilo alma gibi
        public string Goal { get; set; } = "Maintain";

        // kullanıcının tercihi olan diyet türü
        // tarif önerilerinde kullanılıyor
        public string DietType { get; set; } = "Normal";
    }
}
