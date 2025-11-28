using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    // bu controller tariflerle ilgili tüm api uçlarını topladığımız yer
    // listeleme, tekil getir, ekle, güncelle, sil
    // artı olarak tarifin besin değerlerini tekrar hesaplatabildiğimiz ayrı bir endpoint de var
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeApiController : ControllerBase
    {
        // tariflerle ilgili tüm iş mantığı recipeService içinde
        private readonly IRecipeService _recipeService;

        // besin değerleri hesaplamaları için nutritionService
        private readonly INutritionService _nutritionService;

        public RecipeApiController(
            IRecipeService recipeService,
            INutritionService nutritionService)
        {
            _recipeService = recipeService;
            _nutritionService = nutritionService;
        }

        // Get: api/RecipeApi?search=pizza
        // buradan tüm tarifleri çekiyoruz
        // eğer search parametresi verilmişse isim bazlı filtreleme yapıyor
        [HttpGet]
        public async Task<ActionResult<List<Recipe>>> GetAll([FromQuery] string? search = null)
        {
            var recipes = await _recipeService.GetAllAsync(search);
            return Ok(recipes);
        }

        // Get api/RecipeApi/5
        // id ye göre tekil tarif detayını getiriyor
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Recipe>> GetById(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
                return NotFound(); // tarif yoksa 404

            return Ok(recipe);
        }

        // Post: api/RecipeApi
        // yeni tarif oluşturma endpointi
        [HttpPost]
        public async Task<ActionResult<Recipe>> Create([FromBody] Recipe recipe)
        {
            // model validation patlarsa kullanıcıya hata dönüyoruz
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // service tarafında tarifi veritabanına ekliyoruz
            var created = await _recipeService.CreateAsync(recipe);

            // 201 + location header ile dönüyoruz
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Put: api/RecipeApi/5
        // mevcut tarifi güncelleme kısmı
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Recipe>> Update(int id, [FromBody] Recipe recipe)
        {
            // route taki id ile body içindeki id aynı olmalı
            if (id != recipe.Id)
                return BadRequest("Id ile gövdedeki Id uyuşmuyor");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _recipeService.UpdateAsync(recipe);

            // null dönerse tarif bulunamamış demektir
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // Delete: api/RecipeApi/5
        // tarif silme endpointi
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _recipeService.DeleteAsync(id);

            // service false dönerse zaten olmayan bir tarifi silmeye çalışıyoruz demek
            if (!ok)
                return NotFound();

            return NoContent();
        }

        // Post: api/RecipeApi/5/recalculate-nutrition
        // tarif için besin değeri hesaplamasını buradan da tetikleyebiliyoruz
        // özellikle admin panelden manuel güncelleme yapmak istersek işe yarar
        [HttpPost("{id:int}/recalculate-nutrition")]
        public async Task<IActionResult> RecalculateNutrition(int id)
        {
            var nf = await _nutritionService.SaveNutritionForRecipeAsync(id);

            // tarif bulunmazsa ya da hesaplama patlarsa kullanıcıya haber veriyoruz
            if (nf == null)
                return NotFound("Tarif bulunamadı veya hesaplama yapılamadı");

            // başarıyla hesaplandıysa besin değerini döndürüyoruz
            return Ok(nf);
        }
    }
}
