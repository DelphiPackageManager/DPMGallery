using DPMGallery.Entities;
using DPMGallery.Repositories;
using Serilog;
using System.Threading.Tasks;
using System.Threading;
using DPMGallery.Models;
using DPMGallery.Utils;
using MailKit.Search;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace DPMGallery.Services
{
    public class UIService : IUIService
    {
        private readonly ILogger _logger;
        private readonly SearchRepository _searchRepository;
        private readonly IMemoryCache _memoryCache;
        public UIService(ILogger logger, SearchRepository searchRepository, IMemoryCache memoryCache)
        {
            _logger = logger;
            _searchRepository = searchRepository;
            _memoryCache = memoryCache;
        }


        public async Task<PackagesListModel> UISearchAsync(string query = null, int skip = 0, int take = 20,
                                    bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            var searchResponse = await _searchRepository.UISearchAsync(query, skip, take, includePrerelease, includeCommercial,
                                                                     includeTrial, cancellationToken);
            PackagesListModel model = Mapping<UISearchResponse, PackagesListModel>.Map(searchResponse);
            return model;
        }


        public async Task<PackageDetailsModel> UIGetPackageDetails(string packageId, string packageVersion, CancellationToken cancellationToken = default)
        {
            string key = $"{packageId}-{packageVersion}".ToLower();

            //package details are expensive to get so cache for a while
            //one issue is that the IsLatestVersion could become stale, and we have no way to invalidate only
            //a specific packageid (without the version) when a new version is uploaded - so need to investigate this more. 
            var model = await _memoryCache.GetOrCreateAsync<PackageDetailsModel>(key, async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _searchRepository.GetPackageInfo(packageId, packageVersion, cancellationToken);
            });

            return model;
        }
    }
}
