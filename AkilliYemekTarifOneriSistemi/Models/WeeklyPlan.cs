using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Bir kullanicinin belirli bir hafta icin olusturdugu haftalik yemek planini temsil eder.
    /// Ornek: 2025-12-01 tarihinden baslayan hafta icin 7 gun x 4 ogun plan.
    /// </summary>
    public class WeeklyPlan
    {
        /// <summary>
        /// Veritabanı için birincil anahtar (Primary Key).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Bu haftalik plani olusturan kullanicinin kimligi (AspNetUsers.Id).
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Planin basladigi tarih (genellikle haftanin ilk gunu).
        /// Ornek: 2025-12-01 (Pazartesi).
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// UI'da gosterilebilecek kisa bir ad / aciklama.
        /// Ornek: \"Fit Haftalik Plan\", \"Aile Menusu\" gibi.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Navigation property - plani olusturan kullanici.
        /// </summary>
        public IdentityUser User { get; set; } = null!;

        /// <summary>
        /// Bu haftalik plana ait ogun/hucresel kayitlar.
        /// Bir WeeklyPlan, birden cok WeeklyPlanItem icerir (1 - N iliski).
        /// </summary>
        public ICollection<WeeklyPlanItem> Items { get; set; } = new List<WeeklyPlanItem>();
    }
}



