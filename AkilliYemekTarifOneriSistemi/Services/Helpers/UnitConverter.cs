using System;
using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Services.Helpers
{
    public static class UnitConverter
    {
        // ---------------------------------------------
        // YOĞUNLUK TABLOSU (ml → gram dönüşümü için)
        // ---------------------------------------------
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

        // ---------------------------------------------
        // ADET → GRAM DÖNÜŞTÜRME TABLOSU
        // ---------------------------------------------
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

        // ---------------------------------------------
        // ANA DÖNÜŞÜM METODU
        // ---------------------------------------------
        public static double ToGram(double quantity, string unit, string ingredientName)
        {
            if (quantity <= 0)
                return 0;

            unit = unit.ToLower().Trim();
            ingredientName = ingredientName.ToLower().Trim();

            // -----------------------------
            // 1) GRAM
            // -----------------------------
            if (unit is "g" or "gram" or "gr")
                return quantity;

            // -----------------------------
            // 2) KG → GRAM
            // -----------------------------
            if (unit is "kg" or "kilogram" or "kilo")
                return quantity * 1000;

            // -----------------------------
            // 3) ML (Doğrudan Yoğunluk Tabanlı)
            // -----------------------------
            if (unit == "ml")
                return ConvertLiquidToGrams(quantity, ingredientName);

            // -----------------------------
            // 4) LİTRE
            // -----------------------------
            if (unit is "l" or "litre")
                return ConvertLiquidToGrams(quantity * 1000, ingredientName);

            // -----------------------------
            // 5) BARDAK DÖNÜŞÜMLERİ
            // -----------------------------
            if (unit.Contains("su bardağı"))
                return ConvertLiquidToGrams(quantity * 200, ingredientName);

            if (unit.Contains("çay bardağı"))
                return ConvertLiquidToGrams(quantity * 125, ingredientName);

            // -----------------------------
            // 6) KAŞIK DÖNÜŞÜMLERİ
            // -----------------------------
            if (unit.Contains("yemek kaşığı"))
                return ConvertLiquidToGrams(quantity * 15, ingredientName);

            if (unit.Contains("tatlı kaşığı"))
                return ConvertLiquidToGrams(quantity * 7, ingredientName);

            if (unit.Contains("çay kaşığı"))
                return ConvertLiquidToGrams(quantity * 5, ingredientName);

            // -----------------------------
            // 7) ADET
            // -----------------------------
            if (unit == "adet")
                return EstimatePieceWeight(ingredientName) * quantity;

            return 0;
        }

        // ---------------------------------------------
        // YOĞUNLUK İLE ML → GRAM DÖNÜŞÜMÜ
        // ---------------------------------------------
        private static double ConvertLiquidToGrams(double ml, string ingredientName)
        {
            foreach (var kv in DensityTable)
            {
                if (ingredientName.Contains(kv.Key))
                    return ml * kv.Value;
            }

            // eşleşme yoksa SU kabul edilir
            return ml * 1.0;
        }

        // ---------------------------------------------
        // ADET → GRAM
        // ---------------------------------------------
        private static double EstimatePieceWeight(string ingredientName)
        {
            foreach (var kv in PieceWeights)
            {
                if (ingredientName.Contains(kv.Key))
                    return kv.Value;
            }

            return 100; // varsayılan
        }
    }
}
