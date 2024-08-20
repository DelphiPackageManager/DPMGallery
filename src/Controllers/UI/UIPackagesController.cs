using DPMGallery.Models;
using DPMGallery.Types;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using System;
using DPMGallery.Services;
using Serilog;
using Ganss.Xss;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using DPMGallery.Entities;

namespace DPMGallery.Controllers.UI
{

    [ApiController]
    [AllowAnonymous]
    [Route("ui")]
    [DisableRateLimiting]
    public class UIPackagesController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IUIService _uiService;
        private readonly UserManager<User> _userManager;

        public UIPackagesController(ILogger logger, IUIService uiService, UserManager<User> userManager) : base(logger) 
        {
            _logger = logger;
            _uiService = uiService;
            _userManager = userManager;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("packages")]
        [OutputCache(PolicyName = "UIQuery")]
        public async Task<IActionResult> Index(
            [FromQuery] string compiler,
            [FromQuery] string platform,
            [FromQuery] string q,
            [FromQuery] int page = 1,
            [FromQuery] bool prerelease = true,
            [FromQuery] bool commercial = true,
            [FromQuery] bool trial = true,
            [FromQuery] int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
#if DEBUG
                System.Threading.Thread.Sleep(1000); //so we can test the skeleton rendering
#endif
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

                var skip = page > 0 ? (page - 1) * pageSize : 0;

                var model = await _uiService.UISearchAsync(query, skip, pageSize, prerelease, commercial, trial, cancellationToken);

                model.PageSize = pageSize;

                var sanitizer = new HtmlSanitizer();
                //Sanitise any text fields - description etc. 
                foreach (var package in model.Packages)
                {
                    package.Description = sanitizer.Sanitize(package.Description);
                }

                model.Query = query;

                if (model.TotalPackages - (skip + pageSize) > 0)
                {
                    model.NextPage = page + 1;
                }

                if (page > 1)
                {
                    model.PrevPage = Math.Max(1, page - 1);
                }

                return Json(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("packagedetails/{packageId}")]
        [Route("packagedetails/{packageId}/{packageVersion}")]
        [OutputCache(Duration = 10, VaryByRouteValueNames = new string[] { "*" })]
        public async Task<IActionResult> PackageDetails([FromRoute] string packageId, [FromRoute] string packageVersion = "", CancellationToken cancellationToken = default)
        {

            PackageDetailsModel model = await _uiService.UIGetPackageDetails(packageId, packageVersion, cancellationToken: cancellationToken);
            if (model == null)
                return NotFound();

            return Json(model);
        }

        [Authorize]
        [HttpGet]
        [Route("account/packages/published")]
        public async Task<IActionResult> GetListedPackages()
        {
            string userName = HttpContext.User.Identity?.Name;
            if (userName == null)
            {
                //just return nothing
                return Unauthorized();
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("account/packages/unlisted")]
        public async Task<IActionResult> GetUnListedPackages()
        {
            string userName = HttpContext.User.Identity?.Name;
            if (userName == null)
            {
                //just return nothing
                return Unauthorized();
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Unauthorized();
            }





            return Ok();
        }

    }
}
