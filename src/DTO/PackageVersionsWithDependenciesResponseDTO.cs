﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.DTO
{
    public class PackageVersionsWithDependenciesResponseDTO
    {
        public IReadOnlyList<VersionWithDependenciesDTO> Versions { get; set; }
    }
}
