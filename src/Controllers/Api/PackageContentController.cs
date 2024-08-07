﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DPMGallery.DTO;
using DPMGallery.Types;
using DPMGallery.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using DPMGallery.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.OutputCaching;
using DPMGallery.Statistics;

namespace DPMGallery.Controllers.Api
{
    /// <summary>
    /// The Package Content resource, used to download content from packages.
    /// See: https://docs.microsoft.com/en-us/nuget/api/package-base-address-resource
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    public class PackageContentController : Controller
    {
        private readonly IPackageContentService _packageContentService;
        private readonly ISearchService _searchService;
        private readonly IStorageService _storageService;
        private readonly ServerConfig _serverConfig;
        private readonly DownloadsRecordQueue _downloadsRecordQueue;

        public PackageContentController(ServerConfig serverConfig, IPackageContentService packageContentService, ISearchService searchService, 
                                        IStorageService storageService, DownloadsRecordQueue downloadsRecordQueue)
        {
            _serverConfig = serverConfig;
            _packageContentService = packageContentService;
            _searchService = searchService;
            _storageService = storageService;
            _downloadsRecordQueue = downloadsRecordQueue;
        }

        [HttpGet]
        [Route(Constants.RoutePatterns.PackageVersionsWithDeps)]
        [OutputCache(Duration = 30, VaryByRouteValueNames = new[] { "*" })]
        public async Task<ActionResult<PackageVersionsWithDependenciesResponseDTO>> GetPackageVersionsWithDependenciesAsync([FromRoute] string id, [FromRoute] string compilerVersion, [FromRoute] string platform, [FromQuery] string versionRange, [FromQuery(Name = "prerel")] bool includePrerelease, CancellationToken cancellationToken)
        {
            CompilerVersion compiler = compilerVersion.ToCompilerVersion();
            if (compiler == CompilerVersion.UnknownVersion)
                return BadRequest("Invalid Compiler Version");

            Platform thePlatform = platform.ToPlatform();
            if (thePlatform == Platform.UnknownPlatform)
                return BadRequest("Invalid Platform");

            ;

            if (!VersionRange.TryParse(versionRange, out VersionRange range))
            {
                return BadRequest("Invalid Semantic Version");
            }

            return await _searchService.GetPackageVersionsWithDependenciesOrNullAsync(id, compiler, thePlatform, range, includePrerelease,  cancellationToken);
               
        }

        [HttpGet]
        [Route(Constants.RoutePatterns.PackageVersions)]
        [OutputCache(Duration = 30, VaryByRouteValueNames = new[] { "*" })]
        public async Task<ActionResult<PackageVersionsResponseDTO>> GetPackageVersionsAsync(string id, string compilerVersion, string platform, [FromQuery(Name = "prerel")] bool includePrerelease = false, CancellationToken cancellationToken = default)
        {
            CompilerVersion compiler = compilerVersion.ToCompilerVersion();
            if (compiler == CompilerVersion.UnknownVersion)
                return BadRequest("Invalid Compiler Version");

            Platform thePlatform = platform.ToPlatform();
            if (thePlatform == Platform.UnknownPlatform)
                return BadRequest("Invalid Platform");

            var versions = await _packageContentService.GetPackageVersionsOrNullAsync(id, compiler, thePlatform, includePrerelease, cancellationToken);
            if (versions == null)
            {
                return NotFound();
            }

            return versions;
        }

        [HttpGet]
        [Route(Constants.RoutePatterns.PackageInfo)]
        [OutputCache(Duration = 30, VaryByRouteValueNames = new[] {"*"})]
        public async Task<ActionResult<SearchResultDTO>> GetPackageInfo(string id, string compilerVersion, string platform, string version, CancellationToken cancellationToken)
        {
            CompilerVersion compiler = compilerVersion.ToCompilerVersion();
            if (compiler == CompilerVersion.UnknownVersion)
                return BadRequest("Invalid Compiler Version");

            Platform thePlatform = platform.ToPlatform();
            if (thePlatform == Platform.UnknownPlatform)
                return BadRequest("Invalid Platform");

            if (!NuGetVersion.TryParseStrict(version, out _))
            {
                return BadRequest("Invalid Semantic Version");
            }

            //TODO : Validate package id

            var result = await _searchService.GetPackageInfoAsync(id, compiler, thePlatform, version, cancellationToken);

            if (result != null)
                return result;

            return NotFound();

        }

        [HttpGet]
        [Route(Constants.RoutePatterns.PackageDownloadFile)]
        public async Task<IActionResult> DownloadFileAsync(string id, string compilerVersion, string platform, string version, string fileType, CancellationToken cancellationToken)
        {
            CompilerVersion compiler = compilerVersion.ToCompilerVersion();
            if (compiler == CompilerVersion.UnknownVersion)
                return BadRequest("Invalid Compiler Version");

            Platform thePlatform = platform.ToPlatform();
            if (thePlatform == Platform.UnknownPlatform)
                return BadRequest("Invalid Platform");

            if (!NuGetVersion.TryParseStrict(version, out _))
            {
                return BadRequest("Invalid Semantic Version");
            }

            if (!Enum.TryParse(fileType, out DownloadFileType downloadFileType))
                return BadRequest("Invalid file type");


            //check package actually exists!
            bool exists = await _packageContentService.GetPackageVersionExistsAsync(id, version, compiler, thePlatform, cancellationToken);
            if (!exists)
            {
                return NotFound();
            }

			string fileExt = "";
			if (downloadFileType == DownloadFileType.icon)
			{
				fileExt = await _packageContentService.GetPackageIconFileExtAsync(id, compiler, thePlatform, version, cancellationToken);
			}


			//if the storage is a cdn (ie aws or google cloud) then redirect to the cdn url for the package file.
			//note we are returning a 302 temporary redirect as we still want downloads to come here so we can count them
			if (_storageService.IsCDN())
            {
                if (string.IsNullOrEmpty(_serverConfig.Storage.CDNBaseUri))
                {
                    return Problem("CDN Is not configured for storage provider", statusCode: 503);
                }

                if (downloadFileType == DownloadFileType.dpkg)
                {
                    _downloadsRecordQueue.RecordDownload(id, thePlatform, compiler, version);                
                }


                string packageUrl = _serverConfig.Storage.CDNBaseUri?.ToLower();
                if (packageUrl.EndsWith('/'))
                    packageUrl = packageUrl.TrimEnd('/');


				//add path elements

				if (downloadFileType == DownloadFileType.icon)
				{
                    fileType = fileExt.Remove(0, 1).ToLower();
				}


				//make sure the path to the file is lowercase to avoid issues with linux filesystems
				packageUrl = $"{packageUrl}/{compiler.Sanitise()}/{platform}/{id}/{id}-{compiler.Sanitise()}-{platform}-{version}.{fileType}".ToLower();

				return Redirect(packageUrl);
            }
            //we only get here if we are using the filesystem for storage, which is dev only.

            var packageStream = await _packageContentService.GetPackageStreamAsync(downloadFileType, id, compiler, thePlatform, version, cancellationToken);
            if (packageStream == null)
            {
                return NotFound();
            }
            string fileName = $"{id}-{compiler.Sanitise()}-{platform}-{version}".ToLower();

            //TODO : how do we deal with different icon file types? perhaps just stick to png?
            if (downloadFileType == DownloadFileType.icon)
            {
                fileName = fileName + fileExt;
            }
            else
                fileName = fileName + fileType;
            
            return File(packageStream, downloadFileType.ToContentType(fileExt), fileName);
        }

    }
}
