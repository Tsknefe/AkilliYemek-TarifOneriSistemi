using System;
using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Services.Helpers
{
    public static class UnitConverter
    {
        private static readonly Dictionary<string, double> DensityTable = new()
        {
            { "su", 1.0 },
            { "süt", 1.03 },
            { "zeytinyaðý", 0.91 },
            { "ayçiçek yaðý", 0.92 },
            { "tereyaðý", 0.95 },
            { "bal", 1.40 },
            { "un", 0.53 },
            { "þeker", 0.85 }
        };

        private static readonly Dictionary<string, double> PieceWeights = new()
        {
            { "yumurta", 55 },
            { "domates", 150 },
            { "salatalýk", 120 },
            { "patates", 180 },
            { "soðan", 110 },
            { "havuç", 70 },
            { "elma", 130 },
            { "muz", 120 }
        };

        public static double ToGram(double quantity, string unit, string ingredientName)
        {
            if (quantity <= 0)
                return 0;

            unit = unit.ToLower().Trim();
            ingredientName = ingredientName.ToLower().Trim();

            if (unit is "g" or "gram" or "gr")
                return quantity;

            if (unit is "kg" or "kilogram" or "kilo")
                return quantity * 1000;

            if (unit == "ml")
                return ConvertLiquidToGrams(quantity, ingredientName);

            if (unit is "l" or "litre")
                return ConvertLiquidToGrams(quantity * 1000, ingredientName);

            if (unit.Contains("su bardaðý"))
                return ConvertLiquidToGrams(quantity * 200, ingredientName);

            if (unit.Contains("çay bardaðý"))
                return ConvertLiquidToGrams(quantity * 125, ingredientName);

            if (unit.Contains("yemek kaþýðý"))
                return ConvertLiquidToGrams(quantity * 15, ingredientName);

            if (unit.Contains("tatlý kaþýðý"))
                return ConvertLiquidToGrams(quantity * 7, ingredientName);

            if (unit.Contains("çay kaþýðý"))
                return ConvertLiquidToGrams(quantity * 5, ingredientName);

            if (unit == "adet")
                return EstimatePieceWeight(ingredientName) * quantity;

            return 0;
        }

        private static double ConvertLiquidToGrams(double ml, string ingredientName)
        {
            foreach (var kv in DensityTable)
            {
                if (ingredientName.Contains(kv.Key))
                    return ml * kv.Value;
            }

            return ml * 1.0;
        }

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
