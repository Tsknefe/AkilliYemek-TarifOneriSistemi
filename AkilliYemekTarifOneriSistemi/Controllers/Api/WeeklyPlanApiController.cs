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
        
        [HttpGet("generate")]
        public async Task<IActionResult> Generate([FromQuery] DateTime? startDate = null)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var plan = await _weeklyPlanService.GenerateWeeklyPlanAsync(userId, startDate);
            if (plan == null)
                return BadRequest("Profiliniz veya uygun tarif bulunamadý. Lütfen UserProfile bilgilerinizin dolu olduðundan emin olun.");

            return Ok(plan);
        }
    }
}
