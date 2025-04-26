using DPMGallery.Data;
using DPMGallery.DTO;
using DPMGallery.Entities;
using DPMGallery.Repositories;
using DPMGallery.Types;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DPMGallery.Utils;
using NuGet.Versioning;

namespace DPMGallery.Services
{
    public class PackageContentService : IPackageContentService
    {
        private readonly PackageRepository _packageRepository;
        private readonly PackageVersionRepository _packageVersionRepository;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        public PackageContentService(ILogger logger, IStorageService storageService, PackageVersionRepository packageVersionRepository, PackageRepository packageRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _storageService = storageService;
            _packageRepository = packageRepository;
            _packageVersionRepository = packageVersionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PackageVersionsResponseDTO> GetPackageVersionsOrNullAsync(string packageId, CompilerVersion compilerVersion, Platform platform, bool includePrerelease, CancellationToken cancellationToken)
        {
            var versions = await _packageVersionRepository.GetPackageVersionStringsAsync(packageId, compilerVersion, platform, includePrerelease, cancellationToken);

            var result = new PackageVersionsResponseDTO();

            if (versions != null) //
            {
                result.Versions = versions.ToList();
            }

            return result;
        }


        public async Task<bool> GetPackageVersionExistsAsync(string packageId, string version, CompilerVersion compilerVersion, Platform platform, CancellationToken cancellationToken)
        {
            return await _packageVersionRepository.GetPackageVersionExistsAsync(packageId, version, compilerVersion, platform, cancellationToken);
        }

        public async Task<Stream> GetPackageStreamAsync(DownloadFileType fileType, string id, CompilerVersion compilerVersion, Platform platform, string version, CancellationToken cancellationToken)
        {
            //we are using all lowercase paths to avoid issues on linux filesystems.  
            string path = Path.Combine($"{compilerVersion.Sanitise()}", $"{platform.ToString().ToLower()}", $"{id.ToLower()}", $"{id}-{compilerVersion.Sanitise()}-{platform}-{version}.");
            if (fileType == DownloadFileType.icon)
            {
                path = path + "png";
            }
            else
                path = $"{path}{fileType}";

            if (fileType == DownloadFileType.dpkg)
            {
                try
                {
                    var packageVersion = await _packageVersionRepository.GetPackageVersionByPackageIdAsync(id, version, compilerVersion, platform, cancellationToken);
                    if (packageVersion == null)
                    {
                        throw new Exception($"Could not find PackageVersion {id}-{compilerVersion.Sanitise()}-{platform}-{version}");
                    }
                    await _packageVersionRepository.IncrementDownloads(packageVersion, 1, cancellationToken);
                    //TODO : Replace this as it does way too much.
                    await _packageRepository.UpdateDownloads(id, cancellationToken);
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "[PackageContentService] Error updating downloads");
                    //not throwing here as we want the download to continue
                }
            }

            return await _storageService.GetAsync(path, cancellationToken);
        }
        public async Task<string> GetPackageIconFileExtAsync(string id, CompilerVersion compilerVersion, Platform platform, string version, CancellationToken cancellationToken)
        {
            var packageVersion = await _packageVersionRepository.GetPackageVersionByPackageIdAsync(id, version, compilerVersion, platform, cancellationToken);

            return Path.GetExtension(packageVersion.Icon);
        }
    }
}
