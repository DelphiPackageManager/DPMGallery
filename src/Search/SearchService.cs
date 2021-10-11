using DPMGallery.Types;
using DPMGallery.DTO;
using DPMGallery.Entities;
using DPMGallery.Repositories;
using DPMGallery.Utils;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DPMGallery.Services
{
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


        public async Task<SearchResponseDTO> SearchAsync(CompilerVersion compilerVersion, Platform platform, string query = null, bool exact = false, int skip = 0, int take = 20,
                                                   bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            //TODO : Enabled searching by tags or by owner.

            var searchResponse = await _searchRepository.SearchAsync(compilerVersion, platform, query, exact, skip, take, includePrerelease, includeCommercial,
                                                                     includeTrial, cancellationToken);

            return Mapping<ApiSearchResponse, SearchResponseDTO>.Map(searchResponse);
            
        }

        public async Task<UISearchResponseDTO> UISearchAsync(string query = null, int skip = 0, int take = 20,
                                    bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            var searchResponse = await _searchRepository.UISearchAsync(query, skip, take, includePrerelease, includeCommercial,
                                                                     includeTrial, cancellationToken);

            return Mapping<UISearchResponse, UISearchResponseDTO>.Map(searchResponse);

        }

    }
}
