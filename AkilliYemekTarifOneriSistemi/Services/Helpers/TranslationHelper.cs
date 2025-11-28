using System.Collections.Generic;

namespace AkilliYemekTarifOneriSistemi.Services.Helpers
{
    // elimizdeki türkçe malzeme isimlerini ingilizceye çevirmek için basit bir çeviri tablosu kullandığımız yardımcı sınıf
    // besin değerlerini openfoodfacts gibi ingilizce çalışan API lardan aldığımız için bu çeviri önemli
    public static class TranslationHelper
    {
        // burada türkçe - ingilizce eşleştirmelerini tutuyoruz
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

        // verilen türkçe ismi ingilizceye çeviren metodumuz
        public static string Translate(string name)
        {
            // boş string geldiyse direkt null döndürüyoruz
            if (string.IsNullOrWhiteSpace(name))
                return null;

            // hem küçük harfe çeviriyoruz hem fazlalıkları temizliyoruz
            name = name.ToLower().Trim();

            // birebir eşleşme varsa direkt çevirip döndürüyoruz
            if (Translations.ContainsKey(name))
                return Translations[name];

            // birebir bulamazsak içinde geçen kelimeye göre eşleştirme yapıyoruz
            foreach (var item in Translations)
            {
                // örn "taze domates" → içinde "domates" geçiyor → "tomato"
                if (name.Contains(item.Key))
                    return item.Value;
            }

            // hiçbir eşleşme yoksa çeviri bulunamadı → null
            return null;
        }
    }
}
