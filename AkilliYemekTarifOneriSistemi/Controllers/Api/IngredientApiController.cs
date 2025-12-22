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

        [HttpGet]
        public async Task<ActionResult<List<Ingredient>>> GetAll([FromQuery] string? search = null)
        {
            var list = await _ingredientService.GetAllAsync(search);
            return Ok(list);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Ingredient>> GetById(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);
            if (ingredient == null)
                return NotFound();

            return Ok(ingredient);
        }

        [HttpPost]
        public async Task<ActionResult<Ingredient>> Create([FromBody] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ingredientService.CreateAsync(ingredient); 
            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Ingredient>> Update(int id, [FromBody] Ingredient ingredient)
        {
            if (id != ingredient.Id)
                return BadRequest("Id uyuþmuyor");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ingredientService.UpdateAsync(ingredient);
            return Ok(ingredient);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ingredientService.DeleteAsync(id);
            return NoContent();
        }
    }
}
