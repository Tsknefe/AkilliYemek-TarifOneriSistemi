using System;

namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Haftalik plan icindeki tek bir hucreyi temsil eder.
    /// Ornek: Carsamba - Aksam - \"Firin Tavuk\" tarifi.
    /// </summary>
    public class WeeklyPlanItem
    {
        /// <summary>
        /// Veritabanı için birincil anahtar (Primary Key).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Bu satirin ait oldugu haftalik planin kimligi (WeeklyPlan.Id).
        /// </summary>
        public int WeeklyPlanId { get; set; }

        /// <summary>
        /// Bu ogun icin secilen tarifin kimligi (Recipe.Id).
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Haftanin hangi gunu icin oldugunu belirtir.
        /// System.DayOfWeek enum'u kullanilir (Pazartesi, Sali, vs.).
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Ogun tipi.
        /// Ornek: \"Kahvalti\", \"Ogle\", \"Aksam\", \"Ara Ogun\".
        /// Basitlik icin string tutuluyor, istenirse enum'a cevrilebilir.
        /// </summary>
        public string MealType { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property - ait oldugu haftalik plan.
        /// </summary>
        public WeeklyPlan WeeklyPlan { get; set; } = null!;

        /// <summary>
        /// Navigation property - bu ogunde yenilecek olan tarif.
        /// </summary>
        public Recipe Recipe { get; set; } = null!;
    }
}



