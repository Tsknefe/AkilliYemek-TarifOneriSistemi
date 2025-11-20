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

        /// <summary>
        /// OpenFoodFacts üzerinden bir besin için 100g başına besin değerlerini getirir
        /// </summary>
        /// <param name="name">Malzemenin türkçe adı örneğin domaates</param>
        /// <param name="englishName">
        /// varsa ingilizce adı örneğin Tomato dolu ise öncelik bunda
        /// </param>
        [HttpGet("lookup")]
        public async Task<ActionResult<NutritionFacts>> Lookup(
            [FromQuery] string name,
            [FromQuery] string? englishName = null)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(englishName))
                return BadRequest("En azından name veya englishName parametresi dolu olmalı.");

            var nf = await _nutritionService.GetNutritionAsync(name, englishName);

            if (nf is null)
                return NotFound("Uygun besin değeri bulunamadı.");

            return Ok(nf);
        }

        /// <summary>
        /// Verilen tarif için (RecipeId) tüm malzemelere göre besin değerlerini hesaplar ve veritabanına yazar.
        /// </summary>
        /// <param name="recipeId">Tarif Id</param>
        [HttpPost("recipe/{recipeId:int}/recalculate")]
        public async Task<ActionResult<NutritionFacts>> RecalculateForRecipe(int recipeId)
        {
            if (recipeId <= 0)
                return BadRequest("Geçersiz recipeId.");

            var nf = await _nutritionService.SaveNutritionForRecipeAsync(recipeId);

            if (nf is null)
                return NotFound("Verilen recipeId için tarif bulunamadı veya hesaplama yapılamadı.");

            return Ok(nf);
        }
    }
}
