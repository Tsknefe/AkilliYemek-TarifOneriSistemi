using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeIngredientApiController : ControllerBase
    {
        private readonly IRecipeIngredientService _service;

        public RecipeIngredientApiController(IRecipeIngredientService service)
        {
            _service = service;
        }

        [HttpGet("by-recipe/{recipeId:int}")]
        public async Task<ActionResult<List<RecipeIngredient>>> GetByRecipe(int recipeId)
        {
            var list = await _service.GetByRecipeIdAsync(recipeId);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RecipeIngredient>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<RecipeIngredient>> Add([FromBody] RecipeIngredientCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Quantity <= 0 || string.IsNullOrWhiteSpace(dto.Unit))
                return BadRequest("Miktar ve birim geçerli olmalýdýr");

            var created = await _service.AddAsync(dto.RecipeId, dto.IngredientId, dto.Quantity, dto.Unit);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RecipeIngredient>> Update(int id, [FromBody] RecipeIngredientUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Id uyuþmuyor");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Quantity <= 0 || string.IsNullOrWhiteSpace(dto.Unit))
                return BadRequest("Miktar ve birim geçerli olmalýdýr");

            var updated = await _service.UpdateAsync(dto.Id, dto.IngredientId, dto.Quantity, dto.Unit);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok)
                return NotFound();

            return NoContent();
        }
    }


    public class RecipeIngredientCreateDto
    {
        public int RecipeId { get; set; }

        public int IngredientId { get; set; }

        public double Quantity { get; set; }

        public string Unit { get; set; } = string.Empty;
    }

    public class RecipeIngredientUpdateDto : RecipeIngredientCreateDto
    {
        public int Id { get; set; }
    }
}
