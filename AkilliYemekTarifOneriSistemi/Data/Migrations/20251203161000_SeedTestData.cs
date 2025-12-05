using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AkilliYemekTarifOneriSistemi.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kategoriler ekleniyor
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "Description" },
                values: new object[,]
                {
                    { 1, "Çorba", "Sıcak ve besleyici çorba tarifleri" },
                    { 2, "Ana Yemek", "Doyurucu ana yemek tarifleri" },
                    { 3, "Salata", "Sağlıklı salata tarifleri" },
                    { 4, "Tatlı", "Lezzetli tatlı tarifleri" }
                });

            // Etiketler ekleniyor
            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Kolay" },
                    { 2, "Hızlı" },
                    { 3, "Sağlıklı" },
                    { 4, "Vejetaryen" },
                    { 5, "Protein" }
                });

            // Malzemeler ekleniyor
            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "Id", "Name", "DefaultUnit", "Description" },
                values: new object[,]
                {
                    { 1, "Kırmızı Mercimek", "su bardağı", "Kırmızı mercimek" },
                    { 2, "Soğan", "adet", "Orta boy soğan" },
                    { 3, "Havuç", "adet", "Orta boy havuç" },
                    { 4, "Zeytinyağı", "yemek kaşığı", "Sızma zeytinyağı" },
                    { 5, "Tuz", "çay kaşığı", "İyotlu tuz" },
                    { 6, "Karabiber", "çay kaşığı", "Taze çekilmiş karabiber" },
                    { 7, "Tavuk Göğsü", "gram", "Temizlenmiş tavuk göğsü" },
                    { 8, "Domates", "adet", "Orta boy domates" },
                    { 9, "Biber", "adet", "Yeşil biber" },
                    { 10, "Un", "su bardağı", "Beyaz un" },
                    { 11, "Şeker", "su bardağı", "Toz şeker" },
                    { 12, "Yumurta", "adet", "Büyük boy yumurta" },
                    { 13, "Süt", "ml", "Tam yağlı süt" },
                    { 14, "Marul", "yaprak", "Taze marul yaprakları" },
                    { 15, "Salatalık", "adet", "Orta boy salatalık" }
                });

            // Tarifler ekleniyor
            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "Title", "ShortDescription", "Instructions", "ImageUrl", "PreparationTimeMinutes", "Difficulty", "CategoryId" },
                values: new object[,]
                {
                    { 1, "Kırmızı Mercimek Çorbası", "Geleneksel Türk mutfağının vazgeçilmez çorbası", "1. Mercimeği yıkayıp tencereye alın.\n2. Soğan ve havucu küp küp doğrayıp ekleyin.\n3. Zeytinyağı, tuz ve karabiber ekleyip kaynatın.\n4. Blendırdan geçirip servis edin.", null, 30, "Kolay", 1 },
                    { 2, "Fırında Tavuk", "Pratik ve lezzetli bir tavuk tarifi", "1. Tavuk göğsünü küp küp doğrayın.\n2. Zeytinyağı, tuz ve baharatlarla marine edin.\n3. Fırında 180 derecede 40 dakika pişirin.\n4. Sıcak servis edin.", null, 50, "Orta", 2 },
                    { 3, "Karışık Salata", "Taze ve sağlıklı bir salata", "1. Marul ve salatalığı yıkayıp doğrayın.\n2. Domates ve biberi ekleyin.\n3. Zeytinyağı ve limon suyu ile soslayın.\n4. Tuz ve karabiber ekleyip karıştırın.", null, 15, "Kolay", 3 },
                    { 4, "Sütlaç", "Geleneksel Türk tatlısı", "1. Sütü kaynatın.\n2. Şeker ve pirinci ekleyip pişirin.\n3. Soğutup servis edin.", null, 45, "Kolay", 4 }
                });

            // Tarif-Malzeme ilişkileri (RecipeIngredients)
            migrationBuilder.InsertData(
                table: "RecipeIngredients",
                columns: new[] { "RecipeId", "IngredientId", "Amount", "Unit", "Note" },
                values: new object[,]
                {
                    { 1, 1, 1m, "su bardağı", null },
                    { 1, 2, 1m, "adet", "Küp küp doğranmış" },
                    { 1, 3, 1m, "adet", "Küp küp doğranmış" },
                    { 1, 4, 2m, "yemek kaşığı", null },
                    { 1, 5, 1m, "çay kaşığı", null },
                    { 1, 6, 0.5m, "çay kaşığı", null },
                    { 2, 7, 500m, "gram", null },
                    { 2, 4, 3m, "yemek kaşığı", null },
                    { 2, 5, 1m, "çay kaşığı", null },
                    { 2, 6, 1m, "çay kaşığı", null },
                    { 3, 14, 5m, "yaprak", null },
                    { 3, 15, 2m, "adet", "Dilimlenmiş" },
                    { 3, 8, 2m, "adet", "Küp küp doğranmış" },
                    { 3, 9, 1m, "adet", "Dilimlenmiş" },
                    { 3, 4, 2m, "yemek kaşığı", null },
                    { 4, 13, 1000m, "ml", null },
                    { 4, 11, 0.5m, "su bardağı", null },
                    { 4, 10, 0.25m, "su bardağı", "Pirinç unu" }
                });

            // Tarif-Etiket ilişkileri (RecipeTags)
            migrationBuilder.InsertData(
                table: "RecipeTags",
                columns: new[] { "RecipeId", "TagId" },
                values: new object[,]
                {
                    { 1, 1 }, // Mercimek Çorbası - Kolay
                    { 1, 2 }, // Mercimek Çorbası - Hızlı
                    { 1, 3 }, // Mercimek Çorbası - Sağlıklı
                    { 1, 4 }, // Mercimek Çorbası - Vejetaryen
                    { 2, 1 }, // Fırında Tavuk - Kolay
                    { 2, 5 }, // Fırında Tavuk - Protein
                    { 3, 1 }, // Salata - Kolay
                    { 3, 2 }, // Salata - Hızlı
                    { 3, 3 }, // Salata - Sağlıklı
                    { 3, 4 }, // Salata - Vejetaryen
                    { 4, 1 }  // Sütlaç - Kolay
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Seed data'yı geri almak için silme işlemleri
            migrationBuilder.DeleteData(
                table: "RecipeTags",
                keyColumns: new[] { "RecipeId", "TagId" },
                keyValues: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 2, 1 },
                    { 2, 5 },
                    { 3, 1 },
                    { 3, 2 },
                    { 3, 3 },
                    { 3, 4 },
                    { 4, 1 }
                });

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumns: new[] { "RecipeId", "IngredientId" },
                keyValues: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 5 },
                    { 1, 6 },
                    { 2, 7 },
                    { 2, 4 },
                    { 2, 5 },
                    { 2, 6 },
                    { 3, 14 },
                    { 3, 15 },
                    { 3, 8 },
                    { 3, 9 },
                    { 3, 4 },
                    { 4, 13 },
                    { 4, 11 },
                    { 4, 10 }
                });

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4 });

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5 });

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4 });
        }
    }
}

