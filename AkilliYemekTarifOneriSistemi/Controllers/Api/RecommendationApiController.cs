using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    // bu controller bizim öneri motorunun api tarafı
    // burada iki farklı uç var
    // 1 basic olan sadece parametrelere göre öneri dönen endpoint
    // 2 kullanıcı profiline göre kişisel öneri dönen endpoint
    // react tarafındaki RecommendationPage şu an basic endpointi kullanıyor
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationApiController : ControllerBase
    {
        // öneri ile ilgili asıl hesaplamalar service içinde yapılıyor
        private readonly IRecommendationService _recommendationService;

        public RecommendationApiController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        // 1) Eski klasik parametre bazlı öneri
        // ingredient listesi max süre hedef kalori diyet tipi ve top değerine göre çalışıyor
        [HttpPost("basic")]
        public async Task<IActionResult> RecommendBasic([FromBody] RecommendationRequestDto dto)
        {
            // body tamamen boş gelirse
            if (dto == null)
                return BadRequest("İstek gövdesi boş olamaz");

            // top dolu değilse default 10 veriyoruz
            dto.Top ??= 10;

            // service çağrısı burada yapılıyor
            var results = await _recommendationService.RecommendAsync(
                dto.IngredientIds ?? new List<int>(),
                dto.MaxCookingTime,
                dto.TargetCalories,
                dto.DietType,
                dto.Top.Value);

            // entityyi direkt dönmek yerine frontend için sade bir dto hazırlıyoruz
            var response = results.Select(r => new RecommendationResultDto
            {
                RecipeId = r.Recipe.Id,
                Name = r.Recipe.Name,
                Description = r.Recipe.Description,
                CookingTime = r.Recipe.CookingTime,
                Servings = r.Recipe.Servings,
                DietType = r.Recipe.DietType,

                // skor ve alt kırılımlar
                Score = r.Score,
                IngredientMatch = r.IngredientMatch,
                TimeFit = r.TimeFit,
                CalorieFit = r.CalorieFit,
                DietFit = r.DietFit,
                MacroFit = r.MacroFit,
                ProfileDietFit = r.ProfileDietFit
            }).ToList();

            return Ok(response);
        }

        // 2) kullanıcı profiline göre öneri
        // burada ekstra olarak kullanıcının alerjileri diyet tercihleri vs service tarafında hesaba katılabiliyor
        [HttpPost("for-user")]
        [Authorize] // profil bazlı öneri için kullanıcı login olmalı
        public async Task<IActionResult> RecommendForUser([FromBody] UserRecommendationRequestDto dto)
        {
            if (dto == null)
                return BadRequest("İstek gövdesi boş olamaz");

            dto.Top ??= 10;

            // IdentityUser Id sini claim üzerinden alıyoruz
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Kullanıcı bilgisi bulunamadı");

            // bu sefer service e userId de gönderiyoruz
            var results = await _recommendationService.RecommendForUserAsync(
                userId,
                dto.IngredientIds ?? new List<int>(),
                dto.MaxCookingTime,
                dto.Top.Value);

            // yine aynı result dto sınıfını kullanıyoruz
            var response = results.Select(r => new RecommendationResultDto
            {
                RecipeId = r.Recipe.Id,
                Name = r.Recipe.Name,
                Description = r.Recipe.Description,
                CookingTime = r.Recipe.CookingTime,
                Servings = r.Recipe.Servings,
                DietType = r.Recipe.DietType,

                Score = r.Score,
                IngredientMatch = r.IngredientMatch,
                TimeFit = r.TimeFit,
                CalorieFit = r.CalorieFit,
                DietFit = r.DietFit,
                MacroFit = r.MacroFit,
                ProfileDietFit = r.ProfileDietFit
            }).ToList();

            return Ok(response);
        }
    }

    // burdan sonrası dto tarafı
    // frontend ile konuşurken entity yerine bunları kullanıyoruz daha temiz oluyor

    // basic öneri isteği için kullanılan dto
    public class RecommendationRequestDto
    {
        // kullanıcı hangi malzemeleri seçtiyse onların idleri
        public List<int>? IngredientIds { get; set; }

        // maksimum pişirme süresi dk cinsinden
        public int? MaxCookingTime { get; set; }

        // hedef toplam kalori
        public double? TargetCalories { get; set; }

        // diyet tipi normal vegan vejetaryen vs
        public string? DietType { get; set; }

        // kaç tane sonuç istediği top 5 top 10 gibi
        public int? Top { get; set; }
    }

    // profil bazlı öneri isteği için dto
    public class UserRecommendationRequestDto
    {
        public List<int>? IngredientIds { get; set; }
        public int? MaxCookingTime { get; set; }
        public int? Top { get; set; }
    }

    // sonuçları frontend e dönerken kullandığımız dto
    public class RecommendationResultDto
    {
        // tarif bilgileri
        public int RecipeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CookingTime { get; set; }
        public int Servings { get; set; }
        public string DietType { get; set; } = string.Empty;

        // öneri skoru ve alt skorlar
        public double Score { get; set; }
        public double IngredientMatch { get; set; }
        public double TimeFit { get; set; }
        public double CalorieFit { get; set; }
        public double DietFit { get; set; }

        // makro besin uyumu ve kullanıcının profil diyetiyle uyum skoru
        public double MacroFit { get; set; }
        public double ProfileDietFit { get; set; }
    }
}
