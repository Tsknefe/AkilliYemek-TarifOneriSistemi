using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeApiController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        private readonly INutritionService _nutritionService;

        public RecipeApiController(
            IRecipeService recipeService,
            INutritionService nutritionService)
        {
            _recipeService = recipeService;
            _nutritionService = nutritionService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Recipe>>> GetAll([FromQuery] string? search = null)
        {
            var recipes = await _recipeService.GetAllAsync(search);
            return Ok(recipes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Recipe>> GetById(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
                return NotFound(); 

            return Ok(recipe);
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> Create([FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _recipeService.CreateAsync(recipe);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Recipe>> Update(int id, [FromBody] Recipe recipe)
        {
            if (id != recipe.Id)
                return BadRequest("Id ile gövdedeki Id uyuþmuyor");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _recipeService.UpdateAsync(recipe);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _recipeService.DeleteAsync(id);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{id:int}/recalculate-nutrition")]
        public async Task<IActionResult> RecalculateNutrition(int id)
        {
            var nf = await _nutritionService.SaveNutritionForRecipeAsync(id);

            if (nf == null)
                return NotFound("Tarif bulunamadý veya hesaplama yapýlamadý");

            return Ok(nf);
        }
    }
}
