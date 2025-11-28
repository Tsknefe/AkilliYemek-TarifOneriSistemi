using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    // bu controller tamamen besin değerleri işlemleri için kullanılıyor
    // openfoodfacts üzerinden çekme
    // tarif için toplu besin hesaplama
    // yani sistemdeki nutrition işlemlerinin merkezi burası
    [ApiController]
    [Route("api/[controller]")]
    public class NutritionApiController : ControllerBase
    {
        // service injection
        // bütün iş nutritionService içinde
        private readonly INutritionService _nutritionService;

        public NutritionApiController(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        /// <summary>
        /// OpenFoodFacts üzerinden bir besin için 100g başına besin değerlerini getirir
        /// </summary>
        // bu endpoint dış APIden veri çektiğimiz kısım
        // kullanıcı malzeme ismini giriyor ve biz openfoodfacts'ten besin değerlerini alıyoruz
        // englishName verilirse onu önce kullanıyoruz çünkü çoğu API ing isimle daha iyi sonuç veriyor
        [HttpGet("lookup")]
        public async Task<ActionResult<NutritionFacts>> Lookup(
            [FromQuery] string name,
            [FromQuery] string? englishName = null)
        {
            // hem name hem englishName boş ise hata veriyoruz
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(englishName))
                return BadRequest("En azından name veya englishName parametresi dolu olmalı");

            // service ile API çağrısı
            var nf = await _nutritionService.GetNutritionAsync(name, englishName);

            // veri bulunamazsa 404 dönüyor
            if (nf is null)
                return NotFound("Uygun besin değeri bulunamadı");

            // başarılı ise direk besin değerleri dönüyor
            return Ok(nf);
        }

        /// <summary>
        /// Verilen tarif için (RecipeId) tüm malzemelere göre besin değerlerini hesaplar ve veritabanına yazar.
        /// </summary>
        // burası tarif için otomatik besin değeri hesaplayan endpoint
        // tarifteki tüm malzemeleri dolaşıp hesaplama yapıyor
        // sonra sonuçları NutritionFacts tablosuna kaydediyor
        [HttpPost("recipe/{recipeId:int}/recalculate")]
        public async Task<ActionResult<NutritionFacts>> RecalculateForRecipe(int recipeId)
        {
            // 0 veya altı bir id olursa direkt hata dönüyoruz
            if (recipeId <= 0)
                return BadRequest("Geçersiz recipeId");

            // hesaplama işlemi service içinde yapılıyor
            var nf = await _nutritionService.SaveNutritionForRecipeAsync(recipeId);

            // tarif bulunamadıysa veya hesaplanamadıysa kullanıcıya bildiriyoruz
            if (nf is null)
                return NotFound("Verilen recipeId için tarif bulunamadı veya hesaplama yapılamadı");

            // başarıyla hesaplandıysa sonuç dönüyor
            return Ok(nf);
        }
    }
}
