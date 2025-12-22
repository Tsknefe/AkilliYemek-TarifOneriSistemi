using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    
    
    
    
    [Authorize]
    public class UserAllergiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAllergyService _allergyService;

        public UserAllergiesController(
            ApplicationDbContext context,
            IAllergyService allergyService)
        {
            _context = context;
            _allergyService = allergyService;
        }

        
        
        private string? GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            
            var list = await _allergyService.GetUserAllergiesAsync(userId);

            return View(list);
        }

        
        
        public async Task<IActionResult> Create()
        {
            var ingredients = await _context.Ingredients.ToListAsync();
            ViewBag.Ingredients = ingredients;
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            
            if (ingredientId == 0)
            {
                ModelState.AddModelError("", "Lütfen bir malzeme seçin");
                ViewBag.Ingredients = await _context.Ingredients.ToListAsync();
                return View();
            }

            
            await _allergyService.AddAllergyAsync(userId, ingredientId);

            return RedirectToAction(nameof(Index));
        }

        
        
        public async Task<IActionResult> Delete(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            
            var list = await _allergyService.GetUserAllergiesAsync(userId);

            
            var item = list.FirstOrDefault(x => x.IngredientId == ingredientId);
            if (item == null) return NotFound();

            return View(item);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            
            await _allergyService.RemoveAllergyAsync(userId, ingredientId);

            return RedirectToAction(nameof(Index));
        }
    }
}
