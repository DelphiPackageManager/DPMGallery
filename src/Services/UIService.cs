using DPMGallery.Entities;
using DPMGallery.Repositories;
using DPMGallery.Extensions;
using Serilog;
using System.Threading.Tasks;
using System.Threading;
using DPMGallery.Models;
using DPMGallery.Utils;
using MailKit.Search;
using Microsoft.Extensions.Caching.Memory;
using System;
using DPMGallery.Types;
using static DPMGallery.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DPMGallery.Statistics;
using DPMGallery.Extensions.Mapping;

namespace DPMGallery.Services
{
    public class UIService : IUIService
    {
        private readonly ILogger _logger;
        private readonly SearchRepository _searchRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IStorageService _storageService;
        private readonly ServerConfig _serverConfig;

        public UIService(ILogger logger, ServerConfig serverConfig, SearchRepository searchRepository, IMemoryCache memoryCache, IStorageService storageService)
        {
            _logger = logger;
            _serverConfig = serverConfig;
            _storageService = storageService;
            _searchRepository = searchRepository;
            _memoryCache = memoryCache;
        }


        public async Task<PackagesListModel> UISearchAsync(string query = null, int skip = 0, int take = 20,
                                    bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            var searchResponse = await _searchRepository.UISearchAsync(query, skip, take, includePrerelease, includeCommercial,
                                                                     includeTrial, cancellationToken);
            PackagesListModel model = searchResponse.ToModel();
            return model;
        }

        private string GetDownloadUrl(string packageId, string packageVersion, string compilerVersion, string platform, string fileType)
        {
            return $"/api/v1/package/{packageId}/{compilerVersion}/{platform}/{packageVersion}/{fileType}".ToLower();
        }

        public async Task<PackageDetailsModel> UIGetPackageDetails(string packageId, string packageVersion, CancellationToken cancellationToken = default)
        {


            if (string.IsNullOrEmpty(packageVersion))
            {
                packageVersion = await _searchRepository.GetLatestPackageVersion(packageId, cancellationToken);
            }
            string key = $"{packageId}-{packageVersion}".ToLower();

            //package details are expensive to get so cache for a while
            //one issue is that the IsLatestVersion could become stale, and we have no way to invalidate only
            //a specific packageid (without the version) when a new version is uploaded - so need to investigate this more. 
            var model = await _memoryCache.GetOrCreateAsync<PackageDetailsModel>(key, async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                var result =  await _searchRepository.GetPackageInfo(packageId, packageVersion, cancellationToken);
                if (result == null)
                    return null;

                bool hasIcon = !string.IsNullOrEmpty(result.Icon);
                bool hasReadme = !string.IsNullOrEmpty(result.ReadMe);
                result.Icon = "";
                result.ReadMe = "";
                foreach (var item in result.CompilerPlatforms)
                {
                    foreach (var platform in item.Platforms)
                    {
                        //just get the first icon and readme
                        if (hasIcon && string.IsNullOrEmpty(result.Icon))
                        {
                            result.Icon = GetDownloadUrl(result.PackageId, result.PackageVersion, item.CompilerVersion.ToString(), platform.Platform.ToString(), "icon");
                        }
						if (hasReadme && string.IsNullOrEmpty(result.ReadMe))
						{
							result.ReadMe = GetDownloadUrl(result.PackageId, result.PackageVersion, item.CompilerVersion.ToString(), platform.Platform.ToString(), "readme");
						}

						platform.DownloadUrl = GetDownloadUrl(result.PackageId, result.PackageVersion, item.CompilerVersion.ToString(), platform.Platform.ToString(), "dpkg");
                    }
                }

                return result;
            });

            return model;
        }
    }
}
