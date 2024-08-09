using DPMGallery.Data;
using DPMGallery.Entities;
using DPMGallery.Models.Account;
using System.Collections.Generic;

namespace DPMGallery.Extensions.Mapping
{
    public static class ApiKeyMappings
    {
        public static ApiKeyModel ToModel(this ApiKey entity)
        {
            var model = new ApiKeyModel(entity.Id, entity.Name, entity.Key, entity.UserId, entity.ExpiresUTC, entity.GlobPattern, entity.Packages, entity.PackageOwner, entity.Scopes, entity.Revoked);
            return model;
        }

        public static List<ApiKeyModel> ToModels(this IList<ApiKey> entities)
        {
            if (entities == null)
                return null;

            var items = new List<ApiKeyModel>();
            foreach (ApiKey entity in entities)
                items.Add(entity.ToModel());
            return items;
        }

        public static PagedList<ApiKeyModel> ToPagedModel(this PagedList<ApiKey> entities)
        {
            if (entities == null)
                return null;

            List<ApiKeyModel> items = entities.Items.ToModels();

            if (items == null)
                return null;

            var pagedList = new PagedList<ApiKeyModel>(items, entities.TotalCount, entities.Paging);

            return pagedList;

        }
    }
}
