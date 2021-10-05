using DPMGallery.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DPMGallery.Extensions;

namespace DPMGallery.Models
{
    public class PackagesViewModel
    {
        public long TotalPackages { get; set; } = 25000;
        public string Query { get; set; }
        public string Compiler { get; set; }
        public IReadOnlyList<PackageViewModel> Packages { get; set; } = new List<PackageViewModel>();
    }
}
