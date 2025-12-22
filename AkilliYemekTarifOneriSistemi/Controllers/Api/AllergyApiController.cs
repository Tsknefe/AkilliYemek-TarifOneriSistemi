using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class AllergyApiController : ControllerBase
    {
        private readonly IAllergyService _allergyService;

        public AllergyApiController(IAllergyService allergyService)
        {
            _allergyService = allergyService;
        }

        private string? GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetMyAllergies()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(); 

            var list = await _allergyService.GetUserAllergiesAsync(userId);

            var dto = list.Select(x => new
            {
                x.IngredientId,
                IngredientName = x.Ingredient.Name
            });

            return Ok(dto);
        }

        [HttpPost("{ingredientId:int}")]
        public async Task<IActionResult> Add(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            await _allergyService.AddAllergyAsync(userId, ingredientId);

            return NoContent();
        }

        [HttpDelete("{ingredientId:int}")]
        public async Task<IActionResult> Remove(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            await _allergyService.RemoveAllergyAsync(userId, ingredientId);

            return NoContent();
        }
    }
}
