using DPMGallery.Types;
using DPMGallery.DTO;
using DPMGallery.Models;
using DPMGallery.Entities;
using DPMGallery.Repositories;
using DPMGallery.Utils;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NuGet.Versioning;

namespace DPMGallery.Services
{
    //TODO : this is pointless - move the contents the the call sites (controllers). 
    public class SearchService : ISearchService
    {
        private readonly ILogger _logger;
        private readonly SearchRepository _searchRepository;
        public SearchService(ILogger logger, SearchRepository searchRepository)
        {
            _logger = logger;
            _searchRepository = searchRepository;
        }

        public async Task<ListResponseDTO> ListAsync(CompilerVersion compilerVersion, List<Platform> platforms, string query = null, bool exact = false, int skip = 0, int take = 20,
                                                   bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            //TODO : Enabled searching by tags or by owner.

            var searchResponse = await _searchRepository.ListAsync(compilerVersion, platforms, query, exact, skip, take, includePrerelease, includeCommercial,
                                                                     includeTrial, cancellationToken);

            return Mapping<ApiListResponse, ListResponseDTO>.Map(searchResponse);

        }


        public async Task<SearchResponseDTO> SearchByIdsAsync(CompilerVersion compilerVersion, Platform platform, List<SearchIdDTO> ids, CancellationToken cancellationToken)
        {
            var searchResponse = await _searchRepository.SearchByIdsAsync(compilerVersion, platform, ids, cancellationToken);

            return Mapping<ApiSearchResponse, SearchResponseDTO>.Map(searchResponse);

        }


        public async Task<SearchResponseDTO> SearchAsync(CompilerVersion compilerVersion, Platform platform, string query = null, bool exact = false, int skip = 0, int take = 20,
                                                   bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            //TODO : Enabled searching by tags or by owner.

            var searchResponse = await _searchRepository.SearchAsync(compilerVersion, platform, query, exact, skip, take, includePrerelease, includeCommercial,
                                                                     includeTrial, cancellationToken);

            return Mapping<ApiSearchResponse, SearchResponseDTO>.Map(searchResponse);

        }


        public async Task<SearchResultDTO> GetPackageInfoAsync(string packageId, CompilerVersion compilerVersion, Platform platform, string version, CancellationToken cancellationToken = default)
        {
            var result = await _searchRepository.GetPackageInfo(packageId, compilerVersion, platform, version, cancellationToken);

            if (result == null)
                return null;

            return Mapping<SearchResult, SearchResultDTO>.Map(result);
        }

        public async Task<PackageVersionsWithDependenciesResponseDTO> GetPackageVersionsWithDependenciesOrNullAsync(string packageId, CompilerVersion compilerVersion, Platform platform, VersionRange range, bool includePrerelease, CancellationToken cancellationToken)
        {

            var results = await _searchRepository.GetPackageVersionsWithDependenciesAsync(packageId, compilerVersion, platform, range, includePrerelease, cancellationToken);

            var result = new PackageVersionsWithDependenciesResponseDTO()
            {
                Versions = Mapping<SearchResult, SearchResultDTO>.Map(results)
            };

            return result;
        }

        public async Task<FindResponseDTO> FindAsync(string id, CompilerVersion compilerVersion, Platform platform, string version, bool includePrerelease = true, CancellationToken cancellationToken = default)
        {
            var findResult = await _searchRepository.FindAsync(id, compilerVersion, platform, version, includePrerelease, cancellationToken);

            if (findResult == null)
                return null;

            return Mapping<ApiFindResponse, FindResponseDTO>.Map(findResult);
        }

        public async Task<SearchResultDTO> FindLatestAsync(string id, CompilerVersion compilerVersion, Platform platform, bool includePrerelease = true, CancellationToken cancellationToken = default)
        {
            var findResult = await _searchRepository.FindAsync(id, compilerVersion, platform, "", includePrerelease, cancellationToken);

            if (findResult == null)
                return null;

            return await GetPackageInfoAsync(id, compilerVersion, platform, findResult.Version, cancellationToken);
        }
    }
}
