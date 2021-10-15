using DPMGallery.DTO;
using DPMGallery.Types;
using DPMGallery.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DPMGallery.Controllers
{
    public class SearchController : Controller
    {

        private readonly ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }


        [HttpGet]
        public async Task<ActionResult<ListResponseDTO>> ListAsync(CancellationToken cancellationToken,
            [FromQuery] string compiler,
            [FromQuery] string platforms,
            [FromQuery(Name = "q")] string query = null,
            [FromQuery] bool exact = false,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 40,
            [FromQuery(Name ="prerel")] bool includePrerelease = true,
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
        public async Task<ActionResult<SearchResponseDTO>> SearchByIdsAsync(CancellationToken cancellationToken, [FromBody] SearchByIdRequestDTO model)
        {
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
        public async Task<ActionResult<SearchResponseDTO>> SearchAsync(CancellationToken cancellationToken,
            [FromQuery] string compiler,
            [FromQuery] string platform,
            [FromQuery(Name = "q")] string query = null,
            [FromQuery] bool exact = false,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 40,
            [FromQuery(Name ="prerel")] bool includePrerelease = true,
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
    }
}
