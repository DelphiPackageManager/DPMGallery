using DPMGallery.DTO;
using DPMGallery.Entities;
using DPMGallery.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Search for packages
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="includePrerelease"></param>
        /// <param name="compilerVersions"></param>
        /// <param name="platforms"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResponseDTO> SearchAsync(CompilerVersion compilerVersion, Platform platform, string query = null, bool exact = false, int skip = 0, int take = 20,
                                            bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default);

        Task<SearchResponseDTO> SearchByIdsAsync(CompilerVersion compilerVersion, Platform platform, List<SearchIdDTO> ids, CancellationToken cancellationToken);

        Task<ListResponseDTO> ListAsync(CompilerVersion compilerVersion, List<Platform> platforms, string query = null, bool exact = false, int skip = 0, int take = 20,
                                            bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default);

        Task<SearchResultDTO> GetPackageInfoAsync(string packageId, CompilerVersion compilerVersion, Platform platform, string version, CancellationToken cancellationToken = default);

        Task<PackageVersionsWithDependenciesResponseDTO> GetPackageVersionsWithDependenciesOrNullAsync(string packageId, CompilerVersion compilerVersion, Platform platform, VersionRange range, bool includePrerelease, CancellationToken cancellationToken);

        Task<FindResponseDTO> FindAsync(string id,  CompilerVersion compilerVersion, Platform platform, string version, bool includePrerelease = true, CancellationToken cancellationToken = default);

        Task<SearchResultDTO> FindLatestAsync(string id, CompilerVersion compilerVersion, Platform platform, bool includePrerelease = true, CancellationToken cancellationToken = default);

    }
}
