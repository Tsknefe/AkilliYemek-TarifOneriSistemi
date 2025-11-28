using AkilliYemekTarifOneriSistemi.Models;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    // bu controller bizim malzemelerle ilgili api uçlarımız
    // frontend tarafta malzeme listesi, tekil malzeme, ekleme, güncelleme, silme işlemleri buradan dönüyor
    // kısaca Ingredient için CRUD api diyebiliriz
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientApiController : ControllerBase
    {
        // bütün işi yapan service burası sadece onu çağırıyor
        private readonly IIngredientService _ingredientService;

        public IngredientApiController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        /// <summary>
        /// Sistemde kayıtlı tüm malzemeleri listeler. (isteğe bağlı arama)
        /// </summary>
        // burası tüm malzemeleri getirdiğimiz endpoint
        // querystringten search parametresi gelirse isim bazlı filtreleme yapılabiliyor
        [HttpGet]
        public async Task<ActionResult<List<Ingredient>>> GetAll([FromQuery] string? search = null)
        {
            var list = await _ingredientService.GetAllAsync(search);
            return Ok(list);
        }

        /// <summary>
        /// Id'ye göre tek bir malzemeyi getirir.
        /// </summary>
        // tekil malzeme detayını çekmek için
        // id yoksa 404 dönüyoruz
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
        // yeni bir malzeme kaydı oluşturduğumuz yer
        // model validation geçmezse direkt 400 dönüyor
        [HttpPost]
        public async Task<ActionResult<Ingredient>> Create([FromBody] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // service tarafında veritabanına ekliyoruz
            await _ingredientService.CreateAsync(ingredient); // IIngredientService içinde CreateAsync olması gerekiyor
            // CreatedAtAction ile hem 201 hem de location header dönüyoruz
            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
        }

        /// <summary>
        /// Var olan bir malzemeyi günceller.
        /// </summary>
        // mevcut bir malzemeyi update ettiğimiz endpoint
        // route id ile body içindeki id uyuşmazsa badrequest veriyoruz
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Ingredient>> Update(int id, [FromBody] Ingredient ingredient)
        {
            if (id != ingredient.Id)
                return BadRequest("Id uyuşmuyor");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ingredientService.UpdateAsync(ingredient);
            return Ok(ingredient);
        }

        /// <summary>
        /// Bir malzemeyi siler.
        /// </summary>
        // malzemeyi silmek için kullanılan endpoint
        // service içinden id ye göre silinmesi yeterli burada ekstra kontrol yapılmıyor
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ingredientService.DeleteAsync(id);
            return NoContent();
        }
    }
}
