using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    // bu controller tarif ile malzeme ilişkisini yönettiğimiz yer
    // yani bir tarifin içinde hangi malzemeden ne kadar var hepsini buradan yönetiyoruz
    // ekleme silme güncelleme ve bir tarife ait malzemeleri çekme işlerini bu api yapıyor
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeIngredientApiController : ControllerBase
    {
        // asıl işi yapan service
        private readonly IRecipeIngredientService _service;

        public RecipeIngredientApiController(IRecipeIngredientService service)
        {
            _service = service;
        }

        /// <summary>
        /// Bir tarifteki tüm malzemeleri getirir.
        /// </summary>
        // bir tarifin içindeki tüm RecipeIngredient kayıtlarını çekiyoruz
        // frontend örneğin tarif detay sayfasında burayı kullanabilir
        [HttpGet("by-recipe/{recipeId:int}")]
        public async Task<ActionResult<List<RecipeIngredient>>> GetByRecipe(int recipeId)
        {
            var list = await _service.GetByRecipeIdAsync(recipeId);
            return Ok(list);
        }

        /// <summary>
        /// Tek bir tarif-malzeme kaydını getirir.
        /// </summary>
        // tekil bir RecipeIngredient kaydını id ye göre çekmek için
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RecipeIngredient>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        /// <summary>
        /// Bir tarife yeni malzeme ekler.
        /// </summary>
        // burası bir tarife yeni malzeme eklediğimiz endpoint
        // bodyde RecipeIngredientCreateDto bekliyor
        [HttpPost]
        public async Task<ActionResult<RecipeIngredient>> Add([FromBody] RecipeIngredientCreateDto dto)
        {
            // model validation hata verirse
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // miktar ve birim için biz de ekstra kontrol yapıyoruz
            if (dto.Quantity <= 0 || string.IsNullOrWhiteSpace(dto.Unit))
                return BadRequest("Miktar ve birim geçerli olmalıdır");

            // service içine recipeId ingredientId quantity unit gönderiyoruz
            var created = await _service.AddAsync(dto.RecipeId, dto.IngredientId, dto.Quantity, dto.Unit);

            // 201 Created ile yeni kaydın detayını döndürüyoruz
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Bir tarif-malzeme kaydını günceller.
        /// </summary>
        // burada var olan bir tarif-malzeme kaydını update ediyoruz
        // dto içinde Id de geliyor o yüzden id uyuşmazsa bad request
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RecipeIngredient>> Update(int id, [FromBody] RecipeIngredientUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Id uyuşmuyor");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Quantity <= 0 || string.IsNullOrWhiteSpace(dto.Unit))
                return BadRequest("Miktar ve birim geçerli olmalıdır");

            // güncelleme için service e gönderiyoruz
            var updated = await _service.UpdateAsync(dto.Id, dto.IngredientId, dto.Quantity, dto.Unit);

            // eğer null dönerse zaten böyle bir kayıt yok demek
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        /// <summary>
        /// Bir tarif-malzeme kaydını siler.
        /// </summary>
        // silme işlemi
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok)
                return NotFound();

            return NoContent();
        }
    }

    // burada dto sınıflarını da aynı dosyanın altında tuttuk
    // frontend sadece ihtiyacı olan alanları gönderiyor
    // entity karmakarışık olmasın diye dto üzerinden gidiyoruz

    public class RecipeIngredientCreateDto
    {
        // hangi tarife ekleneceği
        public int RecipeId { get; set; }

        // hangi malzeme olduğu
        public int IngredientId { get; set; }

        // miktar (örneğin 200)
        public double Quantity { get; set; }

        // birim (örneğin gr ml adet vs)
        public string Unit { get; set; } = string.Empty;
    }

    public class RecipeIngredientUpdateDto : RecipeIngredientCreateDto
    {
        // update tarafında ayrıca Id de geliyor
        public int Id { get; set; }
    }
}
