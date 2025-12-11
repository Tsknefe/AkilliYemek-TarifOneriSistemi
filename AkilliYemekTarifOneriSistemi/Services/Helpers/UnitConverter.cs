using System;
using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Services.Helpers
{
    // malzeme miktarlarını gram cinsine çevirmek için yazdığımız yardımcı sınıf
    // besin değerleri hesaplanırken her malzemenin gram değeri gerektiği için bu dönüşümü burada yapıyoruz
    public static class UnitConverter
    {
        // sıvıların ml → gram dönüşümü için yoğunluk tablosu
        // her sıvının yoğunluğu farklı olduğu için su gibi 1:1 değildir
        private static readonly Dictionary<string, double> DensityTable = new()
        {
            { "su", 1.0 },
            { "süt", 1.03 },
            { "zeytinyağı", 0.91 },
            { "ayçiçek yağı", 0.92 },
            { "tereyağı", 0.95 },
            { "bal", 1.40 },
            { "un", 0.53 },
            { "şeker", 0.85 }
        };

        // adet ile ölçülen malzemelerin ortalama gram karşılıkları
        // tariflerde “1 adet yumurta” gibi kullanım çok olduğu için böyle bir tablo gerekli
        private static readonly Dictionary<string, double> PieceWeights = new()
        {
            { "yumurta", 55 },
            { "domates", 150 },
            { "salatalık", 120 },
            { "patates", 180 },
            { "soğan", 110 },
            { "havuç", 70 },
            { "elma", 130 },
            { "muz", 120 }
        };

        // burada tüm dönüşümleri tek bir merkezden yönetiyoruz
        // quantity → miktar
        // unit → birim (örneğin g, kg, ml, su bardağı…)
        // ingredientName → malzeme adı (yoğunluk ve adet dönüşümü için lazım)
        public static double ToGram(double quantity, string unit, string ingredientName)
        {
            if (quantity <= 0)
                return 0;

            unit = unit.ToLower().Trim();
            ingredientName = ingredientName.ToLower().Trim();

            // gram zaten gram ise direk döndürüyoruz
            if (unit is "g" or "gram" or "gr")
                return quantity;

            // kilo → gram
            if (unit is "kg" or "kilogram" or "kilo")
                return quantity * 1000;

            // ml → yoğunluğa göre gram
            if (unit == "ml")
                return ConvertLiquidToGrams(quantity, ingredientName);

            // litre → ml → gram
            if (unit is "l" or "litre")
                return ConvertLiquidToGrams(quantity * 1000, ingredientName);

            // bardak ölçüleri
            if (unit.Contains("su bardağı"))
                return ConvertLiquidToGrams(quantity * 200, ingredientName);

            if (unit.Contains("çay bardağı"))
                return ConvertLiquidToGrams(quantity * 125, ingredientName);

            // yemek kaşığı, tatlı kaşığı, çay kaşığı
            if (unit.Contains("yemek kaşığı"))
                return ConvertLiquidToGrams(quantity * 15, ingredientName);

            if (unit.Contains("tatlı kaşığı"))
                return ConvertLiquidToGrams(quantity * 7, ingredientName);

            if (unit.Contains("çay kaşığı"))
                return ConvertLiquidToGrams(quantity * 5, ingredientName);

            // adet → gram
            if (unit == "adet")
                return EstimatePieceWeight(ingredientName) * quantity;

            return 0;
        }

        // sıvılar için ağırlık dönüşümü
        // burada malzemenin hangi tür sıvı olduğunu density table dan anlamaya çalışıyoruz
        // bulamazsak su kabul edip 1.0 yoğunlukla hesaplıyoruz
        private static double ConvertLiquidToGrams(double ml, string ingredientName)
        {
            foreach (var kv in DensityTable)
            {
                if (ingredientName.Contains(kv.Key))
                    return ml * kv.Value;
            }

            return ml * 1.0;
        }

        // adet ile ölçülen malzemeler için ortalama gram dönüşümü
        private static double EstimatePieceWeight(string ingredientName)
        {
            foreach (var kv in PieceWeights)
            {
                if (ingredientName.Contains(kv.Key))
                    return kv.Value;
            }

            return 100;
        }
    }
}
