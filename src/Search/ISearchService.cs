using DPMGallery.DTO;
using DPMGallery.Entities;
using System;
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
    }
}
