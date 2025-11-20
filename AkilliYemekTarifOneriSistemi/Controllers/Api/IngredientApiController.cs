using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientApiController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientApiController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        /// <summary>
        /// Sistemde kayıtlı tüm malzemeleri listeler.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Ingredient>>> GetAll()
        {
            var list = await _ingredientService.GetAllAsync();
            return Ok(list);
        }

        /// <summary>
        /// Id'ye göre tek bir malzemeyi getirir.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Ingredient>> GetById(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);
            if (ingredient == null)
                return NotFound();

            return Ok(ingredient);
        }

        /// <summary>
        /// Yeni bir malzeme ekler.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Ingredient>> Create([FromBody] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ingredientService.CreateAsync(ingredient); // IIngredientService içinde AddAsync olmalı
            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
        }

        /// <summary>
        /// Var olan bir malzemeyi günceller.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Ingredient>> Update(int id, [FromBody] Ingredient ingredient)
        {
            if (id != ingredient.Id)
                return BadRequest("Id uyuşmuyor.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ingredientService.UpdateAsync(ingredient);
            return Ok(ingredient);
        }

        /// <summary>
        /// Bir malzemeyi siler.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ingredientService.DeleteAsync(id);
            return NoContent();
        }
    }
}
