using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DPMGallery.Data;
using DPMGallery.DTO;
using DPMGallery.Entities;
using DPMGallery.Services;
using Microsoft.AspNetCore.Mvc;
using Semver = SemanticVersioning.Version;

namespace DPMGallery.Controllers.Api
{
    /// <summary>
    /// The Package Content resource, used to download content from packages.
    /// See: https://docs.microsoft.com/en-us/nuget/api/package-base-address-resource
    /// </summary>
    public class PackageContentController : Controller
    {
        private readonly IPackageContentService _packageContentService;
        private readonly IStorageService _storageService;
        private readonly ServerConfig _serverConfig;
        
        public PackageContentController(ServerConfig serverConfig, IPackageContentService packageContentService, IStorageService storageService)
        {
            _serverConfig = serverConfig;
            _packageContentService = packageContentService;
            _storageService = storageService;
        }

        public async Task<ActionResult<PackageVersionsWithDependenciesResponseDTO>> GetPackageVersionsWithDependenciesAsync(string id, string compilerVersion, string platform, CancellationToken cancellationToken)
        {
            CompilerVersion compiler = compilerVersion.ToCompilerVersion();
            if (compiler == CompilerVersion.UnknownVersion)
                return NotFound();

            Platform thePlatform = platform.ToPlatform();
            if (thePlatform == Platform.UnknownPlatform)
                return NotFound();

            var versions = await _packageContentService.GetPackageVersionsWithDependenciesOrNullAsync(id, compiler, thePlatform, cancellationToken);
            if (versions == null)
            {
                return NotFound();
            }

            return versions;
        }



        public async Task<ActionResult<PackageVersionsResponseDTO>> GetPackageVersionsAsync(string id, string compilerVersion, string platform, CancellationToken cancellationToken)
        {
            CompilerVersion compiler = compilerVersion.ToCompilerVersion();
            if (compiler == CompilerVersion.UnknownVersion)
                return NotFound();

            Platform thePlatform = platform.ToPlatform();
            if (thePlatform == Platform.UnknownPlatform)
                return NotFound();

            var versions = await _packageContentService.GetPackageVersionsOrNullAsync(id, compiler, thePlatform, cancellationToken);
            if (versions == null)
            {
                return NotFound();
            }

            return versions;
        }

        public async Task<IActionResult> DownloadFileAsync(string id, string compilerVersion, string platform, string version, string fileType, CancellationToken cancellationToken)
        {
            CompilerVersion compiler = compilerVersion.ToCompilerVersion();
            if (compiler == CompilerVersion.UnknownVersion)
                return NotFound();

            Platform thePlatform = platform.ToPlatform();
            if (thePlatform == Platform.UnknownPlatform)
                return NotFound();

            if (!Semver.TryParse(version, out _))
            {
                return NotFound();
            }

            //check package actually exists!
            bool exists = await _packageContentService.GetPackageVersionExistsAsync(id, version, compiler, thePlatform, cancellationToken);
            if (!exists)
            {
                return NotFound();
            }

            if (!Enum.TryParse(fileType, out DownloadFileType downloadFileType))
                return NotFound();


            //if the storage is a cdn (ie aws or google cloud) then redirect to the cdn url for the package file.
            if (_storageService.IsCDN())
            {
                if (string.IsNullOrEmpty(_serverConfig.Storage.CDNBaseUri))
                {
                    return Problem("CDN Is not configured for storage provider", statusCode: 503);
                }

                string packageUrl = _serverConfig.Storage.CDNBaseUri;
                if (packageUrl.EndsWith('/'))
                    packageUrl = packageUrl.TrimEnd('/');
                //add path elements

                packageUrl = $"{packageUrl}/{compilerVersion}/{platform}/{id}/{id}-{compilerVersion}-{platform}-{version}.{fileType}";

                return Redirect(packageUrl);
            }
            var packageStream = await _packageContentService.GetPackageStreamAsync(downloadFileType, id, compiler, thePlatform, version, cancellationToken);
            if (packageStream == null)
            {
                return NotFound();
            }
            string fileName = $"{id}-{compiler.Sanitise()}-{platform}-{version}.";

            //TODO : how do we deal with different icon file types? perhaps just stick to png?
            if (downloadFileType == DownloadFileType.icon)
            {
                fileName = fileName + "png";
            }
            else
                fileName = fileName + fileType;

            if (downloadFileType == DownloadFileType.dpkg)
            {
                
            }

            return File(packageStream, downloadFileType.ToContentType(), fileName);
        }

    }
}
