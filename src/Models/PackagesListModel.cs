using DPMGallery.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DPMGallery.Extensions;

namespace DPMGallery.Models
{
    public class PackagesListModel
    {
        public long TotalPackages { get; set; }
        public string Query { get; set; }

        public int NextPage { get; set; }

        public int PrevPage { get; set; }

        public IList<PackageListItemModel> Packages { get; set; } = new List<PackageListItemModel>();
    }
}
