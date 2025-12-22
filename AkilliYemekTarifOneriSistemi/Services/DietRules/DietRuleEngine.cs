using System;
using System.Collections.Generic;
using System.Linq;

namespace AkilliYemekTarifOneriSistemi.Services.DietRules
{
    public static class DietRuleEngine
    {
        private static readonly string[] NonVeganKeywords =
        {
            "et","kýyma","tavuk","balýk","somon","hindi","kuzu","dana","sucuk","salam","sosis","pastýrma",
            "meat","beef","chicken","fish","salmon","turkey","lamb","sausage","bacon","ham","salami","pepperoni",

            "süt","peynir","yoðurt","tereyað","kaymak","krema","lor",
            "milk","cheese","yogurt","butter","cream","whey",

            "yumurta","egg",

            "bal","honey","jelatin","gelatin"
        };

        private static readonly string[] NonVegetarianKeywords =
        {
            "et","kýyma","tavuk","balýk","somon","hindi","kuzu","dana","sucuk","salam","sosis","pastýrma",
            "meat","beef","chicken","fish","salmon","turkey","lamb","sausage","bacon","ham","salami","pepperoni"
        };

        private static readonly string[] GlutenKeywords =
        {
            "buðday","un","ekmek","makarna","bulgur","irmik","þehriye","galeta","kraker","arpa","çavdar",
            "wheat","flour","bread","pasta","bulgur","semolina","vermicelli","cracker","breadcrumbs","barley","rye"
        };

        private static readonly string[] HighCarbKeywords =
        {
            "þeker","toz þeker","bal","pekmez","reçel",
            "pirinç","bulgur","makarna","ekmek","un","patates","mýsýr","yulaf",
            "mercimek","nohut","fasulye",
            "sugar","honey","molasses","jam",
            "rice","bulgur","pasta","bread","flour","potato","corn","oat",
            "lentil","chickpea","beans"
        };

        private static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            return s.Trim().ToLowerInvariant();
        }

        private static bool ContainsAny(string text, IEnumerable<string> keywords)
        {
            var t = Normalize(text);
            if (t.Length == 0) return false;
            return keywords.Any(k => t.Contains(Normalize(k)));
        }

        private static List<string> NormalizeTexts(IEnumerable<string> texts)
        {
            return texts
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(Normalize)
                .Where(x => x.Length > 0)
                .Distinct()
                .ToList();
        }

        public static bool IsVegan(IEnumerable<string> texts)
        {
            var list = NormalizeTexts(texts);
            return !list.Any(x => ContainsAny(x, NonVeganKeywords));
        }

        public static bool IsVegetarian(IEnumerable<string> texts)
        {
            var list = NormalizeTexts(texts);
            return !list.Any(x => ContainsAny(x, NonVegetarianKeywords));
        }

        public static bool IsGlutenFree(IEnumerable<string> texts)
        {
            var list = NormalizeTexts(texts);
            return !list.Any(x => ContainsAny(x, GlutenKeywords));
        }

        public static bool IsKetoFriendly(IEnumerable<string> texts)
        {
            var list = NormalizeTexts(texts);

            bool hasHighCarb = list.Any(x => ContainsAny(x, HighCarbKeywords));
            if (hasHighCarb) return false;

            return true;
        }

        public static bool MatchesDiet(string normalizedDiet, IEnumerable<string> texts)
        {
            normalizedDiet = Normalize(normalizedDiet);

            if (normalizedDiet == "" || normalizedDiet == "normal")
                return true;

            return normalizedDiet switch
            {
                "vegan" => IsVegan(texts),
                "vejetaryen" => IsVegetarian(texts),
                "glutensiz" => IsGlutenFree(texts),
                "keto" => IsKetoFriendly(texts),
                _ => true
            };
        }
    }
}
