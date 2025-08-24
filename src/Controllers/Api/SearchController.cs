using DPMGallery.DTO;
using DPMGallery.Types;
using DPMGallery.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace DPMGallery.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [EnableRateLimiting("api-fixed")]
    public class SearchController : Controller
    {

        private readonly ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }


        [HttpGet]
        [Route(Constants.RoutePatterns.PackageList)]
        public async Task<ActionResult<ListResponseDTO>> ListAsync(CancellationToken cancellationToken,
            [FromQuery] string compiler,
            [FromQuery] string platforms,
            [FromQuery(Name = "q")] string query = null,
            [FromQuery] bool exact = false,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 40,
            [FromQuery(Name ="prerel")] bool includePrerelease = false,
            [FromQuery] bool commercial = true,
            [FromQuery] bool trial = true)
        {

            CompilerVersion compilerVersion = CompilerVersion.UnknownVersion;
            if (!string.IsNullOrEmpty(compiler))
            {
                compilerVersion = compiler.ToCompilerVersion();
                if (compilerVersion == CompilerVersion.UnknownVersion)
                    return NotFound();
            }

            var thePlatformStrings = !string.IsNullOrEmpty(platforms) ? platforms.Split(',') : new string[0] ;

            List<Platform> thePlatforms = new();

            foreach (var platformString in thePlatformStrings)
            {
                Platform thePlatform = Platform.UnknownPlatform;

                if (!string.IsNullOrEmpty(platformString.Trim()))
                {
                    thePlatform = platformString.ToPlatform();
                    if (thePlatform == Platform.UnknownPlatform)
                        return NotFound();
                    if (!thePlatforms.Contains(thePlatform))
                        thePlatforms.Add(thePlatform);
                }
            }
            return await _searchService.ListAsync(compilerVersion, thePlatforms, query, exact, skip, take, includePrerelease, commercial, trial, cancellationToken);
        }

        [HttpPost]
        [Route(Constants.RoutePatterns.PackageSearchIds)]
        public async Task<ActionResult<SearchResponseDTO>> SearchByIdsAsync(CancellationToken cancellationToken, [FromBody] SearchByIdRequestDTO model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            //validate model.
            CompilerVersion compilerVersion = CompilerVersion.UnknownVersion;
            if (!string.IsNullOrEmpty(model.Compiler))
            {
                compilerVersion = model.Compiler.ToCompilerVersion();
                if (compilerVersion == CompilerVersion.UnknownVersion)
                    return NotFound();
            }

            Platform thePlatform = Platform.UnknownPlatform;

            if (!string.IsNullOrEmpty(model.Platform))
            {
                thePlatform = model.Platform.ToPlatform();
                if (thePlatform == Platform.UnknownPlatform)
                    return NotFound ();
            }

            return await _searchService.SearchByIdsAsync( compilerVersion, thePlatform, model.PackageIds, cancellationToken);
        }

        [HttpGet]
        [Route(Constants.RoutePatterns.PackageSearch)]
        [OutputCache(Duration = 30, VaryByQueryKeys = new string[] { "compiler", "platform", "q", "exact", "skip", "take", "prerel", "commercial", "istrial" })]
        //TODO : caching isn't working because it doesn't like null values. 
        public async Task<ActionResult<SearchResponseDTO>> SearchAsync(CancellationToken cancellationToken,
            [FromQuery] string compiler,
            [FromQuery] string platform,
            [FromQuery(Name = "q")] string query = null,
            [FromQuery] bool exact = false,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 40,
            [FromQuery(Name ="prerel")] bool includePrerelease = false,
            [FromQuery] bool commercial = true,
            [FromQuery] bool trial = true)
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
            return await _searchService.SearchAsync(compilerVersion, thePlatform, query, exact, skip, take, includePrerelease, commercial, trial, cancellationToken);
        }

        //no longer used, leaving for older clients for now.

        [HttpGet]
        [Route(Constants.RoutePatterns.PackageFind)]
        [OutputCache(Duration = 30, VaryByQueryKeys = new string[] { "id", "compiler", "platform", "version", "prerel"})]
        public async Task<ActionResult<FindResponseDTO>> FindAsync(CancellationToken cancellationToken,
            [FromQuery] string id,
            [FromQuery] string compiler,
            [FromQuery] string platform,
            [FromQuery] string version = null,
            [FromQuery(Name = "prerel")] bool includePrerelease = false)
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



            var result = await _searchService.FindAsync(id, compilerVersion, thePlatform, version, includePrerelease, cancellationToken);

            if (result == null)
                return NotFound();
            return result;

        }


        [HttpGet]
        [Route(Constants.RoutePatterns.PackageFindLatest)]
        [OutputCache(Duration = 60, VaryByQueryKeys = new string[] { "id", "compiler", "platform", "prerel" })]
        public async Task<ActionResult<FindResponseDTO>> FindLatestAsync(CancellationToken cancellationToken,
            [FromQuery] string id,
            [FromQuery] string compiler,
            [FromQuery] string platform,
            [FromQuery(Name = "prerel")] bool includePrerelease = false)
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



            var result = await _searchService.FindAsync(id, compilerVersion, thePlatform, "" , includePrerelease, cancellationToken);

            if (result == null)
                return NotFound();
            return result;

        }



    }
}
