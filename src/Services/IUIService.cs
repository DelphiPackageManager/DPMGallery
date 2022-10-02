using DPMGallery.Entities;
using System.Threading.Tasks;
using System.Threading;
using DPMGallery.Models;

namespace DPMGallery.Services
{
    public interface IUIService
    {
        Task<PackagesListModel> UISearchAsync(string query = null, int skip = 0, int take = 20,
                                    bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default);

        Task<PackageDetailsModel> UIGetPackageDetails(string packageId, string packageVersion, CancellationToken cancellationToken = default);

    }
}
