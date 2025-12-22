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

        public IActionResult Create()
        {
            var today = DateTime.Today;
            var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            var nextMonday = today.AddDays(daysUntilMonday == 0 ? 7 : daysUntilMonday);

            ViewBag.DefaultStartDate = nextMonday;
            return View();
        }

        
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

        
        
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMeal(int weeklyPlanId, int recipeId, DayOfWeek dayOfWeek, string mealType)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var weeklyPlan = await _context.WeeklyPlans
                .FirstOrDefaultAsync(wp => wp.Id == weeklyPlanId && wp.UserId == currentUser.Id);

            if (weeklyPlan == null)
            {
                return NotFound();
            }

            var existingItem = await _context.WeeklyPlanItems
                .FirstOrDefaultAsync(item => item.WeeklyPlanId == weeklyPlanId 
                    && item.DayOfWeek == dayOfWeek 
                    && item.MealType == mealType);

            if (existingItem != null)
            {
                
                existingItem.RecipeId = recipeId;
            }
            else
            {
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

            var shoppingList = GenerateShoppingList(weeklyPlan);

            ViewBag.WeeklyPlan = weeklyPlan;
            return View(shoppingList);
        }

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

        private List<ShoppingListItem> GenerateShoppingList(WeeklyPlan weeklyPlan)
        {
            var ingredientDict = new Dictionary<string, ShoppingListItem>();

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

                    decimal amountInGrams = ConvertToGrams(amount, unit);

                    if (ingredientDict.ContainsKey(ingredientName))
                    {
                        ingredientDict[ingredientName].TotalAmountInGrams += amountInGrams;
                        ingredientDict[ingredientName].UsedInRecipes.Add(recipe.Title);
                    }
                    else
                    {
                        ingredientDict[ingredientName] = new ShoppingListItem
                        {
                            IngredientName = ingredientName,
                            TotalAmountInGrams = amountInGrams,
                            UsedInRecipes = new List<string> { recipe.Title }
                        };
                    }
                }
            }

            var shoppingList = ingredientDict.Values.ToList();
            foreach (var item in shoppingList)
            {
                if (item.TotalAmountInGrams >= 1000)
                {
                    item.DisplayAmount = item.TotalAmountInGrams / 1000m;
                    item.DisplayUnit = "kg";
                }
                else
                {
                    item.DisplayAmount = item.TotalAmountInGrams;
                    item.DisplayUnit = "g";
                }
            }

            return shoppingList.OrderBy(x => x.IngredientName).ToList();
        }

        
        private decimal ConvertToGrams(decimal amount, string unit)
        {
            if (string.IsNullOrEmpty(unit)) return amount;

            unit = unit.ToLower().Trim();

            if (unit == "kg" || unit == "kilogram") return amount * 1000m;
            if (unit == "g" || unit == "gram") return amount;
            if (unit == "mg" || unit == "miligram") return amount / 1000m;

            if (unit == "l" || unit == "litre") return amount * 1000m;
            if (unit == "ml" || unit == "mililitre") return amount;

            if (unit == "yemek kaşığı" || unit == "yk") return amount * 15m; 
            if (unit == "çay kaşığı" || unit == "çk") return amount * 5m; 

            if (unit == "adet")
            {
                
                return amount * 100m; 
            }

            return amount;
        }
    }
}

