using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AkilliYemekTarifOneriSistemi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WeeklyPlanApiController : ControllerBase
    {
        private readonly IWeeklyPlanService _weeklyPlanService;

        public WeeklyPlanApiController(IWeeklyPlanService weeklyPlanService)
        {
            _weeklyPlanService = weeklyPlanService;
        }

        private string? GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        /// <summary>
        /// Giriş yapmış kullanıcı için 7 günlük kişisel yemek planı üretir.
        /// </summary>
        /// <param name="startDate">Opsiyonel; boş ise bugünden başlar.</param>
        [HttpGet("generate")]
        public async Task<IActionResult> Generate([FromQuery] DateTime? startDate = null)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var plan = await _weeklyPlanService.GenerateWeeklyPlanAsync(userId, startDate);
            if (plan == null)
                return BadRequest("Profiliniz veya uygun tarif bulunamadı. Lütfen UserProfile bilgilerinizin dolu olduğundan emin olun.");

            return Ok(plan);
        }
    }
}
