using DPMGallery.Models;
using DPMGallery.Types;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using System;
using DPMGallery.Services;
using Serilog;
using Ganss.Xss;

namespace DPMGallery.Controllers.UI
{
    [ApiController]
    [Route("ui")]
    public class UIPackagesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUIService _uiService;

        public UIPackagesController(ILogger logger, IUIService uiService)
        {
            _logger = logger;
            _uiService = uiService;
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

            var take = 12; //TODO Change to 20 this once we have more packages
            var skip = page > 0 ? (page - 1) * take : 0;

            var model = await _uiService.UISearchAsync(query, skip, take, prerelease, commercial, trial, cancellationToken);

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

        [HttpGet]
        [Route("packagedetails/{packageId}/{packageVersion}")]
        public async Task<IActionResult> PackageDetails([FromRoute] string packageId, [FromRoute] string packageVersion, CancellationToken cancellationToken = default) 
        {

            PackageDetailsModel model = await _uiService.UIGetPackageDetails(packageId, packageVersion, cancellationToken: cancellationToken);
            if (model == null)
                return NotFound();

            return Json(model);
        }

    }
}
