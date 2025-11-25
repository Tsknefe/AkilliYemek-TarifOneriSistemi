using System;

namespace AkilliYemekTarifOneriSistemi.Services.Helpers
{
    public static class UnitConverter
    {
        public static double ToGram(double quantity, string unit, string ingredientName)
        {
            if (quantity <= 0)
                return 0;

            unit = unit.ToLower().Trim();

            // ------------------------------
            // 1) Düz gram birimleri
            // ------------------------------
            if (unit == "g" || unit == "gram" || unit == "gr")
                return quantity;

            // ------------------------------
            // 2) Kilogram -> Gram
            // ------------------------------
            if (unit == "kg" || unit == "kilo" || unit == "kilogram")
                return quantity * 1000;

            // ------------------------------
            // 3) Litre -> ml -> gram (1 ml ≈ 1 g varsayılır)
            // ------------------------------
            if (unit == "l" || unit == "litre")
                return quantity * 1000;

            // ------------------------------
            // 4) Mililitre -> gram
            // ------------------------------
            if (unit == "ml")
                return quantity; // 1 ml = 1 gram kabul

            // ------------------------------
            // 5) Kaşık dönüşümleri
            // ------------------------------
            if (unit.Contains("yemek kaşığı"))
                return quantity * 12;    // ortalama 12g

            if (unit.Contains("tatlı kaşığı"))
                return quantity * 5;

            if (unit.Contains("çay kaşığı"))
                return quantity * 2;

            // ------------------------------
            // 6) Adet için tahmini ağırlıklar
            // ------------------------------
            if (unit == "adet")
            {
                return EstimatePieceWeight(ingredientName) * quantity;
            }

            return 0;
        }


        // ------------------------------
        // 7) Adet birimleri için ağırlık tahmini
        // ------------------------------
        private static double EstimatePieceWeight(string name)
        {
            name = name.ToLower();

            if (name.Contains("yumurta")) return 55;
            if (name.Contains("domates")) return 150;
            if (name.Contains("salatalık")) return 120;
            if (name.Contains("patates")) return 180;
            if (name.Contains("soğan")) return 110;
            if (name.Contains("muz")) return 120;
            if (name.Contains("elma")) return 130;
            if (name.Contains("havuç")) return 70;

            return 100; // varsayılan
        }
    }
}
