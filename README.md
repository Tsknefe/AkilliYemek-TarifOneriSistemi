# Akıllı Yemek & Tarif Öneri Sistemi

ASP.NET Core MVC tabanlı, kullanıcıların elindeki malzemelere ve beslenme tercihlerine göre akıllı tarif ve haftalık menü önerileri yapan bir web uygulaması.

> Teknolojiler: **.NET 9, ASP.NET Core MVC, Entity Framework Core, SQL Server LocalDB, Identity**

---

## 🧱 Genel Mimari

- **ASP.NET Core MVC** (Server-side rendered)
- **Entity Framework Core** (Code First)
- **Identity** ile Authentication & Authorization
- Domain modelleri:
  - `Recipe` (Tarif)
  - `Ingredient` (Malzeme)
  - `RecipeIngredient` (Tarif–Malzeme join tablosu)
  - `NutritionFacts` (Besin değerleri)

İlişkiler:

- `Recipe` – `RecipeIngredient` ➜ **1 - N**
- `Ingredient` – `RecipeIngredient` ➜ **1 - N**
- `Recipe` – `NutritionFacts` ➜ **1 - 1**

Migration dosyaları repoya dahil edildiği için **herkesin kendi makinesinde DB kurması çok kolay**.

---

## ⚙️ Gerekli Araçlar

Projeyi ayağa kaldırmak için gerekenler:

- [.NET SDK 9](https://dotnet.microsoft.com/download)
- **SQL Server LocalDB**  
  Visual Studio ile geliyor. (İstersen normal SQL Server da kullanabilirsin.)
- Visual Studio 2022 **veya** Rider / VS Code

---

## 🚀 Kurulum (Projeyi Çalıştırma)

### 1. Repozitoriyi klonla

```bash
git clone https://github.com/Tsknefe/AkilliYemekTarifOneriSistemi.git
cd AkilliYemekTarifOneriSistemi/AkilliYemekTarifOneriSistemi
