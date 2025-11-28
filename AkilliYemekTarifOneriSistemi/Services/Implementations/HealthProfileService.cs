using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Services.Implementations
{
    // bu service tamamen kullanıcının sağlık profilini hesaplamak için yazıldı
    // burada hedef kalori hesabı, makro besin dağılımı gibi hesaplamaları yapıyoruz
    // recommendation motoru ve weekly plan bu bilgileri kullanıyor

    public class HealthProfileService : IHealthProfileService
    {
        private readonly ApplicationDbContext _context;

        public HealthProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        // kullanıcıya ait profil bilgilerini çekiyoruz
        // eğer profil yoksa controller tarafında otomatik oluşturulmuş oluyor zaten
        public async Task<UserProfile?> GetProfileAsync(string userId)
        {
            return await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // burada BMR hesabı yapıyorum
        // mifflin st jeor formülü daha doğru kabul ediliyor
        // erkek ve kadın için iki farklı formül var
        private double CalculateBmr(UserProfile p)
        {
            if (p.Gender == "Male")
                return 10 * p.WeightKg + 6.25 * p.HeightCm - 5 * p.Age + 5;

            return 10 * p.WeightKg + 6.25 * p.HeightCm - 5 * p.Age - 161;
        }

        // kullanıcının aktivite seviyesine göre çarpan belirliyoruz
        // bu çarpan BMR ile çarpılarak günlük bakım kalorisi bulunuyor
        private double GetActivityMultiplier(string level)
        {
            return level switch
            {
                "sedentary" => 1.2,
                "light" => 1.375,
                "moderate" => 1.55,
                "active" => 1.725,
                "athlete" => 1.9,
                _ => 1.2,
            };
        }

        // burada kullanıcının hedef kalorisini hesaplıyorum
        // önce BMR hesaplanıyor
        // sonra aktivite seviyesi çarpanı uygulanıyor
        // daha sonra kullanıcının hedefine göre +/- 300 kalori ekleniyor
        public async Task<double?> GetTargetCaloriesAsync(string userId)
        {
            var p = await GetProfileAsync(userId);
            if (p is null)
                return null;

            double bmr = CalculateBmr(p);
            double activity = GetActivityMultiplier(p.ActivityLevel);

            double maintenance = bmr * activity;

            return p.Goal switch
            {
                "Lose" => maintenance - 300,      // kilo vermek isteyen için hafif kalori açığı
                "Gain" => maintenance + 300,      // kilo almak isteyen için kalori fazlası
                _ => maintenance                  // maintain için direkt bakım kalorisi
            };
        }

        // burada makro hedeflerini hesaplıyorum
        // protein yağ karbonhidrat dağılımı klasik 25 30 45 olarak ayarladım
        // bu değerler recommendation motorunda tariflere skor verirken kullanılabiliyor
        public async Task<(double proteinGr, double fatGr, double carbGr)?> GetMacroTargetsAsync(string userId)
        {
            var cal = await GetTargetCaloriesAsync(userId);
            if (!cal.HasValue)
                return null;

            double c = cal.Value;

            // 1 gr protein = 4 kalori
            // 1 gr karbonhidrat = 4 kalori
            // 1 gr yağ = 9 kalori
            double proteinCal = c * 0.25;
            double fatCal = c * 0.30;
            double carbCal = c * 0.45;

            return (
                proteinGr: proteinCal / 4,
                fatGr: fatCal / 9,
                carbGr: carbCal / 4
            );
        }
    }
}
