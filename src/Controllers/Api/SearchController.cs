using DPMGallery.DTO;
using DPMGallery.Entities;
using DPMGallery.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

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
