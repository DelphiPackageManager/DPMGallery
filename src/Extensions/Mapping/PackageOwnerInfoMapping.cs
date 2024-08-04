using DPMGallery.Entities;
using DPMGallery.Models;
using DPMGallery.Models.Account;
using System.Collections.Generic;

namespace DPMGallery.Extensions.Mapping
{
    public static class PackageOwnerInfoMapping
    {
        public static PackageOwnerModel ToModel(this PackageOwnerInfo entity)
        {
            return new PackageOwnerModel(entity.UserName, entity.EmailHash);
        }

        public static List<PackageOwnerModel> ToModels(this List<PackageOwnerInfo> entities)
        {
            if (entities == null)
                return null;

            var items = new List<PackageOwnerModel>();
            foreach (PackageOwnerInfo entity in entities)
                items.Add(entity.ToModel());
            return items;
        }

    }
}
