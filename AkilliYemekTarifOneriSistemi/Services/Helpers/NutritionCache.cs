using AkilliYemekTarifOneriSistemi.Models;
using System;
using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Services.Helpers
{
    // dış API'den sürekli veri çekip sistemi yavaşlatmamak için
    // besin değerlerini kısa süreli hafızada tutmak için böyle küçük bir cache yapısı oluşturduk
    public static class NutritionCache
    {
        // cache in içinde key ile eşleşen besin değeri ve ne zaman süre bitecek bilgisi tutuluyor
        private static readonly Dictionary<string, (NutritionFacts facts, DateTime expiry)> Cache =
            new();

        // cache de bir veri var mı diye kontrol ettiğimiz metod
        public static bool TryGet(string key, out NutritionFacts facts)
        {
            facts = null;

            // key varsa kontrol ediyoruz
            if (Cache.ContainsKey(key))
            {
                var (value, time) = Cache[key];

                // eğer süresi dolmamışsa veriyi geri döndürüyoruz
                if (DateTime.Now < time)
                {
                    facts = value;
                    return true;
                }
            }

            // cache de yok ya da süresi dolmuş
            return false;
        }

        // yeni bir besin değerini cache e ekleyen metod
        public static void Set(string key, NutritionFacts facts, int minutes = 60)
        {
            // verinin süresi 60 dakika sonrası olarak ayarlanıyor
            Cache[key] = (facts, DateTime.Now.AddMinutes(minutes));
        }
    }
}
