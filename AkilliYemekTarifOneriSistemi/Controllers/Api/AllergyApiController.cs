using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    // bu controller kullanıcının alerjilerini yönettiğimiz api tarafı
    // react tarafındaki istekler buraya geliyor
    // kullanıcı hangi malzemeye alerjisi olduğunu ekleyebiliyor silebiliyor
    // ayrıca kendi alerji listesini çekebiliyor
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // tüm endpointler login zorunlu
    public class AllergyApiController : ControllerBase
    {
        // alerjiyle ilgili asıl işi yapan service
        private readonly IAllergyService _allergyService;

        public AllergyApiController(IAllergyService allergyService)
        {
            _allergyService = allergyService;
        }

        // şu an login olan kullanıcının id sini çekiyoruz
        // asp net identity nin NameIdentifier claimini kullanıyoruz
        private string? GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        // kullanıcının kendi alerji listesi
        // frontendde profil sayfası veya ayarlar kısmı burayı kullanabilir
        [HttpGet]
        public async Task<IActionResult> GetMyAllergies()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(); // login değilse direkt 401 dön

            // service üzerinden kullanıcının alerji kayıtlarını alıyoruz
            var list = await _allergyService.GetUserAllergiesAsync(userId);

            // dışarıya direkt entity dönmek yerine küçük bir anonim dto döndürüyoruz
            // hem ingredientId hem de ingredient name işimize yarıyor
            var dto = list.Select(x => new
            {
                x.IngredientId,
                IngredientName = x.Ingredient.Name
            });

            return Ok(dto);
        }

        // kullanıcının alerji listesine yeni bir malzeme eklediği nokta
        // örneğin frontend buraya POST /api/AllergyApi/5 gibi bir istek atıyor
        [HttpPost("{ingredientId:int}")]
        public async Task<IActionResult> Add(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            // ilgili kullanıcı için bu ingredientId yi alerji listesine ekliyoruz
            await _allergyService.AddAllergyAsync(userId, ingredientId);

            // ekstra veri dönmemize gerek yok sadece başarılı olduğunu belirtiyoruz
            return NoContent();
        }

        // kullanıcının alerji listesinden bir malzemeyi kaldırdığı nokta
        // DELETE /api/AllergyApi/5 gibi bir istek geliyor
        [HttpDelete("{ingredientId:int}")]
        public async Task<IActionResult> Remove(int ingredientId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            // ilgili ingredientı kullanıcının alerjilerinden siliyoruz
            await _allergyService.RemoveAllergyAsync(userId, ingredientId);

            return NoContent();
        }
    }
}
