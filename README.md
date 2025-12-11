# 🍽️ Akıllı Yemek ve Tarif Öneri Sistemi  

**ASP.NET Core MVC + Entity Framework Core + SQL Server** kullanılarak geliştirilmiş akıllı yemek tarifi öneri ve haftalık menü planlama uygulaması.

Sistem, kullanıcıların evdeki malzemelerine, diyet tercihlerine, kalori ihtiyaçlarına ve tarif hazırlama sürelerine göre akıllı öneriler sunar.  
Ayrıca haftalık yemek planı oluşturabilir, favori tarifleri yönetebilir ve alışveriş listesi üretebilir.

---

# 📌 Özellikler

### ✔ Kullanıcı Yönetimi
- Kayıt / Giriş / Çıkış
- Role-based Authorization (Admin – User)
- Identity tabanlı oturum yönetimi

### ✔ Tarif Yönetimi (CRUD)
- Tarif ekleme / düzenleme / silme / görüntüleme
- Tarife malzeme ekleme (RecipeIngredient)
- Tarif arama & filtreleme

### ✔ Malzeme Yönetimi (CRUD)
- Ingredient ekleme, düzenleme, silme
- Miktar & birim yönetimi

### ✔ Besin Değeri Analizi
- OpenFoodFacts API entegrasyonu
- Kalori, protein, yağ, karbonhidrat, şeker, lif, sodyum değeri hesaplama
- NutritionFacts tablosuna otomatik kayıt

### ✔ Akıllı Tarif Öneri Motoru
- Kullanıcının evdeki malzemelerine göre öneri
- Diyet tipi uyumu
- Kalori uyumu
- Süre uyumu
- Toplam “recommendation score” algoritması

### ✔ Haftalık Yemek Planı
- 7 gün × 3 öğün planlama
- Tarif tekrarını azaltan algoritma
- Kalori ve diyet tipi hedeflerine göre plan oluşturma

### ✔ Favoriler & Alışveriş Listesi
- User – Recipe arasında **N-N** ilişki
- Haftalık plan → Alışveriş listesi üretme
- Birim dönüştürme (ör. 500g + 0.5kg = 1kg birleştirme)
- CSV export

### ✔ Raporlama
- QuestPDF ile haftalık plan PDF çıktısı
- Alışveriş listesini CSV olarak indirme

---

# 🧱 Proje Mimari Yapısı

```text
AkilliYemekTarifOneriSistemi/
│
├── Models/
│   ├── Recipe.cs
│   ├── Ingredient.cs
│   ├── RecipeIngredient.cs
│   └── NutritionFacts.cs
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Services/
│   ├── Interfaces/
│   │   ├── IRecipeService.cs
│   │   ├── IIngredientService.cs
│   │   ├── INutritionService.cs
│   │   └── IWeeklyPlanService.cs
│   └── Implementations/
│       ├── RecipeService.cs
│       ├── IngredientService.cs
│       ├── NutritionService.cs
│       └── WeeklyPlanService.cs
│
├── Controllers/
├── Views/
└── Migrations/
