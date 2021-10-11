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
        public async Task<ListResponseDTO> ListAsync(CancellationToken cancellationToken,
            [FromQuery] string compiler,
            [FromQuery] string platforms,
            [FromQuery(Name = "q")] string query = null,
            [FromQuery] bool exact = false,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 40,
            [FromQuery] bool prerelease = true,
            [FromQuery] bool commercial = true,
            [FromQuery] bool trial = true)
        {

            CompilerVersion compilerVersion = CompilerVersion.UnknownVersion;
            if (!string.IsNullOrEmpty(compiler))
            {
                compilerVersion = compiler.ToCompilerVersion();
                if (compilerVersion == CompilerVersion.UnknownVersion)
                    NotFound();
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
                        NotFound();
                    if (!thePlatforms.Contains(thePlatform))
                        thePlatforms.Add(thePlatform);
                }
            }
            return await _searchService.ListAsync(compilerVersion, thePlatforms, query, exact, skip, take, prerelease, commercial, trial, cancellationToken);
        }



        [HttpGet]
        public async Task<SearchResponseDTO> SearchAsync(CancellationToken cancellationToken,
            [FromQuery] string compiler,
            [FromQuery] string platform,
            [FromQuery(Name = "q")] string query = null,
            [FromQuery] bool exact = false,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 40,
            [FromQuery] bool prerelease = true,
            [FromQuery] bool commercial = true,
            [FromQuery] bool trial = true)
        {
            CompilerVersion compilerVersion = CompilerVersion.UnknownVersion;
            if (!string.IsNullOrEmpty(compiler))
            {
                compilerVersion = compiler.ToCompilerVersion();
                if (compilerVersion == CompilerVersion.UnknownVersion)
                    NotFound();
            }

            Platform thePlatform = Platform.UnknownPlatform;

            if (!string.IsNullOrEmpty(platform))
            {
                thePlatform = platform.ToPlatform();
                if (thePlatform == Platform.UnknownPlatform)
                    NotFound();
            }
            return await _searchService.SearchAsync(compilerVersion, thePlatform, query, exact, skip, take, prerelease, commercial, trial, cancellationToken);
        }
    }
}
