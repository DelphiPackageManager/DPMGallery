﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace DPMGallery.Services
{
    /// <summary>
    /// The result of attempting to index a package.
    /// See <see cref="IPackageIndexingService.IndexAsync(Stream, CancellationToken)"/>.
    /// </summary>
    public enum PackageIndexingStatus
    {
        /// <summary>
        /// An internal error occurred.
        /// </summary>
        Error, 

        /// <summary>
        /// Api key does not have required scope
        /// </summary>
        Forbidden,

        /// <summary>
        /// The package is malformed. 
        /// </summary>
        InvalidPackage,

        FailedAVScan, 


        /// <summary>
        /// The package has already been indexed.
        /// </summary>
        PackageAlreadyExists,

        /// <summary>
        /// The package has been indexed successfully.
        /// </summary>
        Success,
    }

    public class PackageIndexingResult
    {
        public PackageIndexingStatus Status { get; set; }
        public string Message { get; set; }
    }


    public interface IPackageIndexService
    {
        Task<PackageIndexingResult> IndexAsync(Stream stream, int userId,  int apiKeyId, CancellationToken cancellationToken);

        Task<bool> TryDeletePackageAsync(string id, NuGetVersion version, CancellationToken cancellationToken);
    }
}