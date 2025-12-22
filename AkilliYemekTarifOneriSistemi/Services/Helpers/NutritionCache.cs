using AkilliYemekTarifOneriSistemi.Models;
using System;
using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Services.Helpers
{
    public static class NutritionCache
    {
        private static readonly Dictionary<string, (NutritionFacts facts, DateTime expiry)> Cache =
            new();

        public static bool TryGet(string key, out NutritionFacts facts)
        {
            facts = null;

            if (Cache.ContainsKey(key))
            {
                var (value, time) = Cache[key];

                if (DateTime.Now < time)
                {
                    facts = value;
                    return true;
                }
            }

            return false;
        }

        public static void Set(string key, NutritionFacts facts, int minutes = 60)
        {
            Cache[key] = (facts, DateTime.Now.AddMinutes(minutes));
        }
    }
}
