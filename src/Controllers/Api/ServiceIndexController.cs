using DPMGallery.DTO;
using DPMGallery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Controllers
{
    /// <summary>
    /// The DPM Service Index. This helps dpm clients discover this server's services.
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    
    public class ServiceIndexController : Controller
    {
        private readonly IServiceIndexService _serviceIndexService;
        public ServiceIndexController(IServiceIndexService serviceIndexService)
        {
            _serviceIndexService = serviceIndexService;

        }

        [HttpGet]
        [Route("api/v1/index.json")]
        [OutputCache(Duration = 600)] //can be cached for as long as we want, it never changes while the app is running.
        [DisableRateLimiting] //don't rate limit here as it's cached and doesn't hit the db
        public async Task<ServiceIndexResponseDTO> GetAsync(CancellationToken cancellationToken)
        {
            return await _serviceIndexService.GetAsync(cancellationToken);
        }
    }
}