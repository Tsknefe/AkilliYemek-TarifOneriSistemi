using AkilliYemekTarifOneriSistemi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🏠 Ana Sayfa
        public async Task<IActionResult> Index()
        {
            // 🔥 SON EKLENEN 3 TARİF
            var latestRecipes = await _context.Recipes
                .OrderByDescending(r => r.Id)
                .Take(3)
                .ToListAsync();

            return View(latestRecipes);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
