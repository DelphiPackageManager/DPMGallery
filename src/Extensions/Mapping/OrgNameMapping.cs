using DPMGallery.Entities;
using DPMGallery.Models;
using System.Collections.Generic;

namespace DPMGallery.Extensions.Mapping
{
    public static class OrgNameMapping
    {
        public static OrgNameIdModel ToModel(this OrgName entity)
        {
            return new OrgNameIdModel(entity.Id, entity.Name);
        }

        public static List<OrgNameIdModel> ToModels(this List<OrgName> entities)
        {

            if (entities == null)
                return null;

            var items = new List<OrgNameIdModel>();
            foreach (OrgName entity in entities)
                items.Add(entity.ToModel());
            return items;
        }
    }
}
