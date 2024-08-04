using DPMGallery.Entities;
using DPMGallery.Models;

namespace DPMGallery.Extensions.Mapping
{
    public static class UISearchResponseMappings
    {
        public static PackagesListModel ToModel(this UISearchResponse entity)
        {
            return new PackagesListModel()
            {
                TotalPackages = entity.TotalCount,
                Packages = entity.searchResults.ToModel()
            };

        }
    }
}
