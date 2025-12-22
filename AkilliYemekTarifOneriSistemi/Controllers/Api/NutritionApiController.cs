using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class NutritionApiController : ControllerBase
    {
        private readonly INutritionService _nutritionService;

        public NutritionApiController(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        [HttpGet("lookup")]
        public async Task<ActionResult<NutritionFacts>> Lookup(
            [FromQuery] string name,
            [FromQuery] string? englishName = null)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(englishName))
                return BadRequest("En azýndan name veya englishName parametresi dolu olmalý");

            var nf = await _nutritionService.GetNutritionAsync(name, englishName);

            if (nf is null)
                return NotFound("Uygun besin deðeri bulunamadý");

            return Ok(nf);
        }

        [HttpPost("recipe/{recipeId:int}/recalculate")]
        public async Task<ActionResult<NutritionFacts>> RecalculateForRecipe(int recipeId)
        {
            if (recipeId <= 0)
                return BadRequest("Geçersiz recipeId");

            var nf = await _nutritionService.SaveNutritionForRecipeAsync(recipeId);

            if (nf is null)
                return NotFound("Verilen recipeId için tarif bulunamadý veya hesaplama yapýlamadý");

            return Ok(nf);
        }
    }
}
