using DPMGallery.Entities;
using DPMGallery.Models;
using System.Collections.Generic;

namespace DPMGallery.Extensions.Mapping
{
    public static class PackageDependencyMappings
    {
        public static PackageDependencyModel ToModel(this PackageDependency entity)
        {
            return new PackageDependencyModel()
            {
                PackageId = entity.PackageId,
                VersionRange = entity.VersionRange,
            };
        }

        public static List<PackageDependencyModel> ToModel(this List<PackageDependency> entities)
        {
            if (entities == null)
                return null;

            var items = new List<PackageDependencyModel>();
            foreach (PackageDependency entity in entities)
                items.Add(entity.ToModel());
            return items;
        }
    }
}
