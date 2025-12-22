using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Services.Helpers
{
    public static class TranslationHelper
    {
        private static readonly Dictionary<string, string> Translations = new()
        {
            { "domates", "tomato" },
            { "salatalýk", "cucumber" },
            { "soðan", "onion" },
            { "sarýmsak", "garlic" },
            { "patates", "potato" },
            { "havuç", "carrot" },
            { "biber", "pepper" },
            { "patlýcan", "eggplant" },
            { "kabak", "zucchini" },
            { "ýspanak", "spinach" },

            { "elma", "apple" },
            { "muz", "banana" },
            { "portakal", "orange" },
            { "üzüm", "grape" },
            { "çilek", "strawberry" },

            { "süt", "milk" },
            { "yoðurt", "yogurt" },
            { "peynir", "cheese" },
            { "tereyaðý", "butter" },

            { "tavuk", "chicken" },
            { "kýyma", "ground beef" },
            { "balýk", "fish" },
            { "yumurta", "egg" },

            { "pirinç", "rice" },
            { "un", "flour" },
            { "makarna", "pasta" },
            { "bulgur", "bulgur" },

            { "zeytinyaðý", "olive oil" },
            { "ayçiçek yaðý", "sunflower oil" },

            { "tuz", "salt" },
            { "karabiber", "black pepper" },

            { "þeker", "sugar" },
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
