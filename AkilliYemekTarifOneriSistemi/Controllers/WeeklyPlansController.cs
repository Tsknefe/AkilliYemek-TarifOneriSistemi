using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    /// <summary>
    /// Haftalık yemek planı (WeeklyPlan) işlemlerini yöneten controller.
    /// Melisa'nın sorumluluğu: Haftalık planlama UI.
    /// </summary>
    [Authorize]
    public class WeeklyPlansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WeeklyPlansController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public WeeklyPlansController(ApplicationDbContext context, ILogger<WeeklyPlansController> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Kullanıcının tüm haftalık planlarını listeler.
        /// GET: /WeeklyPlans
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var weeklyPlans = await _context.WeeklyPlans
                .Where(wp => wp.UserId == currentUser.Id)
                .OrderByDescending(wp => wp.StartDate)
                .ToListAsync();

            return View(weeklyPlans);
        }

        /// <summary>
        /// Belirli bir haftalık planın detay sayfasını gösterir (7 gün x öğünler tablosu).
        /// GET: /WeeklyPlans/Details/5
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var weeklyPlan = await _context.WeeklyPlans
                .Include(wp => wp.Items)
                    .ThenInclude(item => item.Recipe)
                .FirstOrDefaultAsync(wp => wp.Id == id && wp.UserId == currentUser.Id);

            if (weeklyPlan == null)
            {
                return NotFound();
            }

            // Tüm tarifleri dropdown için getir
            ViewBag.Recipes = await _context.Recipes
                .OrderBy(r => r.Title)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Title
                })
                .ToListAsync();

            return View(weeklyPlan);
        }

        /// <summary>
        /// Yeni haftalık plan oluşturma formunu gösterir.
        /// GET: /WeeklyPlans/Create
        /// </summary>
        public IActionResult Create()
        {
            // Başlangıç tarihi için bugünden itibaren ilk pazartesi
            var today = DateTime.Today;
            var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            var nextMonday = today.AddDays(daysUntilMonday == 0 ? 7 : daysUntilMonday);

            ViewBag.DefaultStartDate = nextMonday;
            return View();
        }

        /// <summary>
        /// Yeni haftalık plan oluşturma işlemini gerçekleştirir.
        /// POST: /WeeklyPlans/Create
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDate,Title")] WeeklyPlan weeklyPlan)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                weeklyPlan.UserId = currentUser.Id;
                _context.Add(weeklyPlan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Haftalık plan başarıyla oluşturuldu! Şimdi tarifler ekleyebilirsiniz.";
                return RedirectToAction(nameof(Details), new { id = weeklyPlan.Id });
            }

            ViewBag.DefaultStartDate = weeklyPlan.StartDate;
            return View(weeklyPlan);
        }

        /// <summary>
        /// Haftalık plana yeni bir öğün (tarif) ekler.
        /// POST: /WeeklyPlans/AddMeal
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMeal(int weeklyPlanId, int recipeId, DayOfWeek dayOfWeek, string mealType)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Planın kullanıcıya ait olduğunu kontrol et
            var weeklyPlan = await _context.WeeklyPlans
                .FirstOrDefaultAsync(wp => wp.Id == weeklyPlanId && wp.UserId == currentUser.Id);

            if (weeklyPlan == null)
            {
                return NotFound();
            }

            // Aynı gün ve öğün için zaten bir tarif var mı kontrol et
            var existingItem = await _context.WeeklyPlanItems
                .FirstOrDefaultAsync(item => item.WeeklyPlanId == weeklyPlanId 
                    && item.DayOfWeek == dayOfWeek 
                    && item.MealType == mealType);

            if (existingItem != null)
            {
                // Varsa güncelle
                existingItem.RecipeId = recipeId;
            }
            else
            {
                // Yoksa yeni ekle
                var newItem = new WeeklyPlanItem
                {
                    WeeklyPlanId = weeklyPlanId,
                    RecipeId = recipeId,
                    DayOfWeek = dayOfWeek,
                    MealType = mealType
                };
                _context.WeeklyPlanItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Tarif plana eklendi!";
            return RedirectToAction(nameof(Details), new { id = weeklyPlanId });
        }

        /// <summary>
        /// Haftalık plandan bir öğünü (tarifi) kaldırır.
        /// POST: /WeeklyPlans/RemoveMeal
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMeal(int itemId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var item = await _context.WeeklyPlanItems
                .Include(i => i.WeeklyPlan)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null || item.WeeklyPlan.UserId != currentUser.Id)
            {
                return NotFound();
            }

            _context.WeeklyPlanItems.Remove(item);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Tarif plandan kaldırıldı!";
            return RedirectToAction(nameof(Details), new { id = item.WeeklyPlanId });
        }

        /// <summary>
        /// Haftalık plan silme onay sayfasını gösterir.
        /// GET: /WeeklyPlans/Delete/5
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var weeklyPlan = await _context.WeeklyPlans
                .Include(wp => wp.Items)
                .FirstOrDefaultAsync(wp => wp.Id == id && wp.UserId == currentUser.Id);

            if (weeklyPlan == null)
            {
                return NotFound();
            }

            return View(weeklyPlan);
        }

        /// <summary>
        /// Haftalık plan silme işlemini gerçekleştirir.
        /// POST: /WeeklyPlans/Delete/5
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var weeklyPlan = await _context.WeeklyPlans
                .FirstOrDefaultAsync(wp => wp.Id == id && wp.UserId == currentUser.Id);

            if (weeklyPlan != null)
            {
                _context.WeeklyPlans.Remove(weeklyPlan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Haftalık plan başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Haftalık plandan alışveriş listesi oluşturur.
        /// Tüm tariflerin malzemelerini toplar, birim dönüştürür ve birleştirir.
        /// GET: /WeeklyPlans/ShoppingList/5
        /// </summary>
        public async Task<IActionResult> ShoppingList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var weeklyPlan = await _context.WeeklyPlans
                .Include(wp => wp.Items)
                    .ThenInclude(item => item.Recipe)
                        .ThenInclude(r => r.RecipeIngredients)
                            .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(wp => wp.Id == id && wp.UserId == currentUser.Id);

            if (weeklyPlan == null)
            {
                return NotFound();
            }

            // Tüm malzemeleri topla ve birleştir
            var shoppingList = GenerateShoppingList(weeklyPlan);

            ViewBag.WeeklyPlan = weeklyPlan;
            return View(shoppingList);
        }

        /// <summary>
        /// Alışveriş listesini CSV olarak indirir.
        /// GET: /WeeklyPlans/ExportCSV/5
        /// </summary>
        public async Task<IActionResult> ExportCSV(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var weeklyPlan = await _context.WeeklyPlans
                .Include(wp => wp.Items)
                    .ThenInclude(item => item.Recipe)
                        .ThenInclude(r => r.RecipeIngredients)
                            .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(wp => wp.Id == id && wp.UserId == currentUser.Id);

            if (weeklyPlan == null)
            {
                return NotFound();
            }

            var shoppingList = GenerateShoppingList(weeklyPlan);

            // CSV oluştur
            var csv = new StringBuilder();
            csv.AppendLine("Malzeme,Miktar,Birim");
            foreach (var item in shoppingList)
            {
                csv.AppendLine($"{item.IngredientName},{item.DisplayAmount},{item.DisplayUnit}");
            }

            var fileName = $"Alisveris_Listesi_{weeklyPlan.StartDate:yyyyMMdd}.csv";
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", fileName);
        }

        /// <summary>
        /// Haftalık plandan alışveriş listesi oluşturur.
        /// Malzemeleri toplar, birim dönüştürür ve birleştirir (500g + 0.5kg = 1kg gibi).
        /// </summary>
        private List<ShoppingListItem> GenerateShoppingList(WeeklyPlan weeklyPlan)
        {
            var ingredientDict = new Dictionary<string, ShoppingListItem>();

            // Tüm plan item'larındaki tariflerin malzemelerini topla
            foreach (var planItem in weeklyPlan.Items ?? new List<WeeklyPlanItem>())
            {
                var recipe = planItem.Recipe;
                if (recipe?.RecipeIngredients == null) continue;

                foreach (var recipeIngredient in recipe.RecipeIngredients)
                {
                    var ingredient = recipeIngredient.Ingredient;
                    var ingredientName = ingredient.Name;
                    var amount = recipeIngredient.Amount;
                    var unit = recipeIngredient.Unit ?? ingredient.DefaultUnit ?? "g";

                    // Birim dönüştürme: Her şeyi gram'a çevir
                    decimal amountInGrams = ConvertToGrams(amount, unit);

                    if (ingredientDict.ContainsKey(ingredientName))
                    {
                        // Zaten varsa miktarı topla
                        ingredientDict[ingredientName].TotalAmountInGrams += amountInGrams;
                        ingredientDict[ingredientName].UsedInRecipes.Add(recipe.Title);
                    }
                    else
                    {
                        // Yeni ekle
                        ingredientDict[ingredientName] = new ShoppingListItem
                        {
                            IngredientName = ingredientName,
                            TotalAmountInGrams = amountInGrams,
                            UsedInRecipes = new List<string> { recipe.Title }
                        };
                    }
                }
            }

            // Görüntüleme için birim dönüştür (gram -> kg, ml -> L vb.)
            var shoppingList = ingredientDict.Values.ToList();
            foreach (var item in shoppingList)
            {
                if (item.TotalAmountInGrams >= 1000)
                {
                    // 1000g ve üzeri -> kg'a çevir
                    item.DisplayAmount = item.TotalAmountInGrams / 1000m;
                    item.DisplayUnit = "kg";
                }
                else
                {
                    // 1000g altı -> gram olarak göster
                    item.DisplayAmount = item.TotalAmountInGrams;
                    item.DisplayUnit = "g";
                }
            }

            return shoppingList.OrderBy(x => x.IngredientName).ToList();
        }

        /// <summary>
        /// Farklı birimleri gram'a çevirir.
        /// </summary>
        private decimal ConvertToGrams(decimal amount, string unit)
        {
            if (string.IsNullOrEmpty(unit)) return amount;

            unit = unit.ToLower().Trim();

            // Ağırlık birimleri
            if (unit == "kg" || unit == "kilogram") return amount * 1000m;
            if (unit == "g" || unit == "gram") return amount;
            if (unit == "mg" || unit == "miligram") return amount / 1000m;

            // Hacim birimleri (yaklaşık olarak 1ml = 1g su için)
            if (unit == "l" || unit == "litre") return amount * 1000m;
            if (unit == "ml" || unit == "mililitre") return amount;

            // Yemek kaşığı, çay kaşığı (yaklaşık)
            if (unit == "yemek kaşığı" || unit == "yk") return amount * 15m; // 1 yk ≈ 15g
            if (unit == "çay kaşığı" || unit == "çk") return amount * 5m; // 1 çk ≈ 5g

            // Adet bazlı tahminler (yaklaşık)
            if (unit == "adet")
            {
                // Adet için ortalama bir tahmin yapılamaz, bu yüzden olduğu gibi bırak
                // Gerçek uygulamada malzeme tipine göre farklı ağırlıklar kullanılabilir
                return amount * 100m; // Varsayılan: 1 adet = 100g
            }

            // Bilinmeyen birimler için olduğu gibi döndür
            return amount;
        }
    }
}

