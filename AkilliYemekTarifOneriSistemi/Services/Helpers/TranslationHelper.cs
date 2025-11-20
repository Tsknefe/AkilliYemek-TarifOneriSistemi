using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Services.Helpers
{
    public static class TranslationHelper
    {
        private static readonly Dictionary<string, string> Translations = new()
        {
            { "domates", "tomato" },
            { "salatalık", "cucumber" },
            { "soğan", "onion" },
            { "sarımsak", "garlic" },
            { "patates", "potato" },
            { "havuç", "carrot" },
            { "biber", "pepper" },
            { "patlıcan", "eggplant" },
            { "kabak", "zucchini" },
            { "ıspanak", "spinach" },

            { "elma", "apple" },
            { "muz", "banana" },
            { "portakal", "orange" },
            { "üzüm", "grape" },
            { "çilek", "strawberry" },

            { "süt", "milk" },
            { "yoğurt", "yogurt" },
            { "peynir", "cheese" },
            { "tereyağı", "butter" },

            { "tavuk", "chicken" },
            { "kıyma", "ground beef" },
            { "balık", "fish" },
            { "yumurta", "egg" },

            { "pirinç", "rice" },
            { "un", "flour" },
            { "makarna", "pasta" },
            { "bulgur", "bulgur" },

            { "zeytinyağı", "olive oil" },
            { "ayçiçek yağı", "sunflower oil" },

            { "tuz", "salt" },
            { "karabiber", "black pepper" },

            { "şeker", "sugar" },
            { "bal", "honey" }
        };

        public static string Translate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            name = name.ToLower().Trim();

            if (Translations.ContainsKey(name))
                return Translations[name];

            foreach (var item in Translations)
            {
                if (name.Contains(item.Key))
                    return item.Value;
            }

            return null;
        }
    }
}
