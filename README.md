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
- Total “recommendation score” algoritması

### ✔ Haftalık Yemek Planı
- 7 gün × 4 öğün planlama
- Tarif tekrarını azaltan algoritma
- Kalori ve diyet tipi hedeflerine göre plan oluşturma

### ✔ Favoriler & Alışveriş Listesi
- User – Recipe arasında **N-N** ilişki
- Haftalık plan → Alışveriş listesi üretme
- Birim dönüştürme (500g + 0.5kg = 1kg birleştirme)
- CSV export

### ✔ Raporlama
- QuestPDF ile haftalık plan PDF çıktısı
- Alışveriş listesini CSV olarak indirme

---

# 🧱 Proje Mimari Yapısı

AkilliYemekTarifOneriSistemi/
│
├── Models/
│ ├── Recipe.cs
│ ├── Ingredient.cs
│ ├── RecipeIngredient.cs
│ └── NutritionFacts.cs
│
├── Data/
│ └── ApplicationDbContext.cs
│
├── Services/
│ ├── Interfaces/
│ │ ├── IRecipeService.cs
│ │ ├── IIngredientService.cs
│ │ ├── INutritionService.cs
│ │ └── IWeeklyPlanService.cs
│ └── Implementations/
│ ├── RecipeService.cs
│ ├── IngredientService.cs
│ ├── NutritionService.cs
│ └── WeeklyPlanService.cs
│
├── Controllers/
├── Views/
└── Migrations/


---

# ⚙️ Gereksinimler

- .NET SDK **9.0+**
- SQL Server **LocalDB** (VS ile otomatik gelir)
- Visual Studio 2022 / VS Code / Rider
- EF Core CLI (dotnet-ef)

---

# 🚀 Kurulum

## 1️⃣ Repozitoriyi Klonla
```bash

git clone https://github.com/Tsknefe/AkilliYemekTarifOneriSistemi.git
cd AkilliYemekTarifOneriSistemi/AkilliYemekTarifOneriSistemi

2️⃣ Veritabanı Ayarını Kontrol Et
appsettings.json içinde:


"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AkilliYemekTarifOneriSistemi;Trusted_Connection=True;MultipleActiveResultSets=true"
}
Eğer farklı SQL Server kullanıyorsan:


"DefaultConnection": "Server=.;Database=AkilliYemekTarifOneriSistemi;Trusted_Connection=True;"

3️⃣ Migration’ları Uygula (DB’yi oluştur)

dotnet tool update --global dotnet-ef
dotnet ef database update
Bu işlem:

Identity tablolarını

Recipes, Ingredients, RecipeIngredients, NutritionFacts tablolarını
tamamen otomatik oluşturur.

4️⃣ Uygulamayı Çalıştır
bash
dotnet run
Tarayıcıdan aç:


http://localhost:5000
https://localhost:5001
🧪 SQL Server’da Veritabanı Görünmüyorsa
SSMS’i aç

Server Name →

scss
(localdb)\MSSQLLocalDB
Databases → sağ tık → Refresh

AkilliYemekTarifOneriSistemi DB’si görünmelidir
---

# 📌 Şu Ana Kadar Yapılanlar (Backend & Frontend Ayrımıyla)

Bu bölüm projede şu ana kadar tamamlanan işleri ve sonraki aşamalarda hangi ekip üyesinin hangi bölümü geliştireceğini açıkça belirtir.

---

## 🟦 Backend – Yapılanlar

### ✔ Backend Proje Altyapısı
- ASP.NET Core MVC backend çatısı kuruldu.
- Entity Framework Core eklendi ve yapılandırıldı.
- Identity sistemi kuruldu (register, login, roller hazır).

### ✔ Domain Modelleri Oluşturuldu
Aşağıdaki backend modelleri yazıldı:
- `Recipe`
- `Ingredient`
- `RecipeIngredient`
- `NutritionFacts`

### ✔ Veritabanı İlişkileri Kuruldu
- Recipe – RecipeIngredient → **1 – N**
- Ingredient – RecipeIngredient → **1 – N**
- Recipe – NutritionFacts → **1 – 1**

### ✔ DbContext Tamamen Hazır
- Fluent API ile tüm ilişkiler kuruldu.
- DbSet’ler eklendi.
- Cascade davranışları tanımlandı.

### ✔ Migration ve Veritabanı Oluşturma
- `InitialCreate` migration hazırlandı.
- `dotnet ef database update` ile **tüm tablolar** oluşturuldu.

### ✔ Recipe CRUD Backend Tamamlandı
- Tarif ekleme / düzenleme / silme / listeleme backend tarafında çalışıyor.
- Validation kuralları eklendi.
- Arama (Search) backend mantığı yazıldı.

---

## 🟧 Frontend – Yapılanlar

### ✔ MVC View Altyapısı Hazır
- Razor View sistemi aktif hale getirildi.
- Layout (navbar–footer) temel yapı oluşturuldu.

### ✔ Backend Testi İçin Scaffold View’lar Üretildi
- Recipe CRUD test sayfaları scaffold edildi.
- Backend fonksiyonlarının çalıştığı doğrulandı.

### ✔ Bootstrap Entegre
- Responsiveness sağlandı (frontend gelişimine hazır hale getirildi).

---

# 🧭 Bundan Sonra Kim Nereyi Yapacak? (Net Görev Dağılımı)

## 🟦 BACKEND EKİBİ

### 👤 **Efe (Backend Lead)**
Sorumluluklar:
- Genel backend mimarisi  
- Recommendation Engine (akıllı tarif öneri algoritması)  
- Weekly Plan motoru (haftalık menü algoritması)  
- Service Layer (DI – SOLID)  
- API endpoint planlama  
- Backend code review  

---

### 👤 **Gül (EF Core & Database)**
Sorumluluklar:
- Ingredient / RecipeIngredient işlemleri  
- Favori sistemi için Many-to-Many model  
- WeeklyPlan için veritabanı modelleri  
- Admin için kategori–etiket modelleri  
- Migration süreçlerinin yönetimi  

---

### 👤 **Emre (Raporlama & API Entegrasyonları)**
Sorumluluklar:
- OpenFoodFacts API ile besin değerlerini çekme  
- NutritionFacts otomatik oluşturma servisi  
- PDF export (QuestPDF)  
- CSV / Excel export  
- Alışveriş listesi backend üretimi  

---

## 🟧 FRONTEND EKİBİ

### 👤 **Melisa (Frontend Lead)**
Sorumluluklar:
- Tarif listeleme ekranı (Card/Grid UI)
- Tarif detay ekranı
- Ingredient CRUD ekranları
- Favoriler UI
- Haftalık planlama UI
- Navbar / Layout düzeni
- Responsive tasarım  

---

### 👤 **Alper (Frontend Developer – UI & UX)**
Sorumluluklar:
- Formlar, tablolar, buton tasarımları
- Arama + filtreleme UI
- Admin panel UI
- Favoriler ekleme butonu
- Shopping list UI
- PDF/CSV download butonları
- Mobil uyum düzenlemeleri  

---

# 🟩 Ortak Yapılacaklar (Backend + Frontend Birlikte)
- Haftalık planlama ekranı (UI + backend algoritma)  
- Favorilere ekleme (UI + backend)  
- Shopping list (UI + backend)  
- Kullanıcı doğrulama akışının iyileştirilmesi  
- PDF/CSV export butonlarının frontend entegrasyonu  

---
