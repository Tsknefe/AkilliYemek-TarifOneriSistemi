using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationApiController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationApiController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpPost("basic")]
        public async Task<IActionResult> RecommendBasic([FromBody] RecommendationRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Ýstek gövdesi boþ olamaz");

            dto.Top ??= 10;

            var results = await _recommendationService.RecommendAsync(
                dto.IngredientIds ?? new List<int>(),
                dto.MaxCookingTime,
                dto.TargetCalories,
                dto.DietType,
                dto.Top.Value);

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

        [HttpPost("for-user")]
        [Authorize] 
        public async Task<IActionResult> RecommendForUser([FromBody] UserRecommendationRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Ýstek gövdesi boþ olamaz");

            dto.Top ??= 10;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Kullanýcý bilgisi bulunamadý");

            var results = await _recommendationService.RecommendForUserAsync(
                userId,
                dto.IngredientIds ?? new List<int>(),
                dto.MaxCookingTime,
                dto.Top.Value);

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


    public class RecommendationRequestDto
    {
        public List<int>? IngredientIds { get; set; }

        public int? MaxCookingTime { get; set; }

        public double? TargetCalories { get; set; }

        public string? DietType { get; set; }

        public int? Top { get; set; }
    }

    public class UserRecommendationRequestDto
    {
        public List<int>? IngredientIds { get; set; }
        public int? MaxCookingTime { get; set; }
        public int? Top { get; set; }
    }

    public class RecommendationResultDto
    {
        public int RecipeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? CookingTime { get; set; }
        public int? Servings { get; set; }
        public string DietType { get; set; } = string.Empty;

        public double Score { get; set; }
        public double IngredientMatch { get; set; }
        public double TimeFit { get; set; }
        public double CalorieFit { get; set; }
        public double DietFit { get; set; }

        public double MacroFit { get; set; }
        public double ProfileDietFit { get; set; }
    }
}
