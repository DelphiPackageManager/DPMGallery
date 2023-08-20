using DPMGallery.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace DPMGallery.Controllers.UI
{
    [ApiController]
    [AllowAnonymous]
    [Route("ui")]
    [DisableRateLimiting] //safe as we cache
    public class StatisticsController : Controller
    {
        private readonly StatisticsData _statisticsData;
        public StatisticsController(StatisticsData statisticsData)
        {
            _statisticsData = statisticsData;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("stats")]
        //[OutputCache(Duration = 30)]
        public IActionResult Index()
        {

            return Json(_statisticsData);
        }
    }
}
