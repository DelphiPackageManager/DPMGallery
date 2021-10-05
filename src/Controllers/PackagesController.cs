using DPMGallery.Entities;
using DPMGallery.Extensions;
using DPMGallery.Models;
using DPMGallery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Controllers
{
    public class PackagesController : Controller
    {
        private readonly ILogger _logger;
        private readonly ISearchService _searchService;
        public PackagesController(ILogger logger, ISearchService searchService)
        {
            _logger = logger;
            _searchService = searchService;
        }

        public async Task<IActionResult> Index([FromQuery] string compiler,
            [FromQuery] string platform,
            [FromQuery(Name = "q")] string query,
            [FromQuery] bool exact = false,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 40,
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
                    NotFound();
            }

            Platform thePlatform = Platform.UnknownPlatform;

            if (!string.IsNullOrEmpty(platform))
            {
                thePlatform = platform.ToPlatform();
                if (thePlatform == Platform.UnknownPlatform)
                    NotFound();
            }


            var seachResults = await _searchService.SearchAsync(compilerVersion, thePlatform, query, exact, skip, take, prerelease, commercial, trial, cancellationToken);

            var model = new PackagesViewModel();
            model.Compiler = compiler;
            model.TotalPackages = seachResults.TotalHits;
            model.Query = query;

            ViewBag.Compiler = "11.0";
            var versions = new List<SelectListItem>();
            foreach (var ver in Enum.GetValues<CompilerVersion>())
            {
                if (ver == CompilerVersion.UnknownVersion)
                    continue;

                var item = new SelectListItem
                {
                    Value = ver.GetDescription(),
                    Text = "Delphi " + ver.GetDescription()
                };
                item.Selected = item.Value == compiler;
                versions.Add(item);
            }
            ViewBag.CompilerVersions = versions;


            return View(model);
        }

        [Authorize]
        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult Details(string id, string compilerVersion, string platform, string version)
        {
            return Content("package details here");
        }
    }
}