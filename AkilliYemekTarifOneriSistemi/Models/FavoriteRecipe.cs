using System;
using Microsoft.AspNetCore.Identity;

namespace AkilliYemekTarifOneriSistemi.Models
{
    /// <summary>
    /// Bir kullanicinin favori olarak isaretledigi tarifleri tutan ara tablo.
    /// User (IdentityUser) ile Recipe arasindaki N-N iliskiyi temsil eder.
    /// Her satir, belirli bir kullanicinin belirli bir tarifi favoriye ekledigini gosterir.
    /// </summary>
    public class FavoriteRecipe
    {
        /// <summary>
        /// Favoriyi olusturan kullanicinin kimligi (Identity tablosundaki Id).
        /// Identity tarafinda AspNetUsers.Id kolonuna karsilik gelir.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Favori yapilan tarifin kimligi (Recipe tablosundaki Id).
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Bu favorinin ne zaman olusturulduguna dair zaman bilgisi (istege bagli).
        /// UI tarafinda siralama veya gosterim icin kullanilabilir.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property - favoriyi olusturan kullanici.
        /// Identity tarafinda AspNetUsers tablosundaki kayda denk gelir.
        /// </summary>
        public IdentityUser User { get; set; } = null!;

        /// <summary>
        /// Navigation property - favori yapilan tarif.
        /// </summary>
        public Recipe Recipe { get; set; } = null!;
    }
}


