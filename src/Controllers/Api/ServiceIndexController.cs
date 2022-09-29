using DPMGallery.DTO;
using DPMGallery.Services;
using Microsoft.AspNetCore.Mvc;
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
    public class ServiceIndexController : Controller
    {
        private readonly IServiceIndexService _serviceIndexService;
        public ServiceIndexController(IServiceIndexService serviceIndexService)
        {
            _serviceIndexService = serviceIndexService;

        }

        [HttpGet]
        [Route("api/v1/index.json")]
        public async Task<ServiceIndexResponseDTO> GetAsync(CancellationToken cancellationToken)
        {
            return await _serviceIndexService.GetAsync(cancellationToken);
        }
    }
}