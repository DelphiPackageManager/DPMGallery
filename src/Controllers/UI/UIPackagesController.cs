using DPMGallery.DTO;
using DPMGallery.Models;
using DPMGallery.Types;
using DPMGallery.Utils;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using System;
using DPMGallery.Services;
using Serilog;

namespace DPMGallery.Controllers.UI
{
    [ApiController]
    [Route("api/ui")]
    public class UIPackagesController : Controller
    {
        private readonly ILogger _logger;
        private readonly ISearchService _searchService;
        public UIPackagesController(ILogger logger, ISearchService searchService)
        {
            _logger = logger;
            _searchService = searchService;
        }


        [HttpGet]
        [Route("packages")]
        public async Task<IActionResult> Index(
            [FromQuery] string compiler,
            [FromQuery] string platform,
            [FromQuery] string q,
            [FromQuery] int page = 1,
            [FromQuery] bool prerelease = true,
            [FromQuery] bool commercial = true,
            [FromQuery] bool trial = true,
            CancellationToken cancellationToken = default)
        {
            CompilerVersion compilerVersion = CompilerVersion.UnknownVersion;
            if (!string.IsNullOrEmpty(compiler))
            {
                compilerVersion = compiler.ToCompilerVersion();
                if (compilerVersion == CompilerVersion.UnknownVersion)
                    return NotFound();
            }

            Platform thePlatform = Platform.UnknownPlatform;

            if (!string.IsNullOrEmpty(platform))
            {
                thePlatform = platform.ToPlatform();
                if (thePlatform == Platform.UnknownPlatform)
                    return NotFound();
            }


            var query = (q ?? string.Empty).Trim();

            //borrowed from nuget - we use sql params anyway but filter out sql injection attempts            
            if (query.ToLowerInvariant().Contains("char(")
                || query.ToLowerInvariant().Contains("union select")
                || query.ToLowerInvariant().Contains("/*")
                || query.ToLowerInvariant().Contains("--"))
            {
                return BadRequest();
            }


            var take = 15;
            var skip = page > 0 ? (page - 1) * take : 0;

            var seachResults = await _searchService.UISearchAsync(query, skip, take, prerelease, commercial, trial, cancellationToken);

            //TODO : mapping from entity to dto to model is wasteful - have the service just return the model? 
            PackagesListModel model = Mapping<UISearchResponseDTO, PackagesListModel>.Map(seachResults);

            var sanitizer = new HtmlSanitizer();
            //Sanitise any text fields - description etc. 
            foreach (var package in model.Packages)
            {
                package.Description = sanitizer.Sanitize(package.Description);

            }

            model.Query = query;

            if (model.TotalPackages - (skip + take) > 0)
            {
                model.NextPage = page + 1;
            }

            if (page > 1)
            {
                model.PrevPage = Math.Max(1, page - 1);
            }

            return Json(model);
        }
    }
}
