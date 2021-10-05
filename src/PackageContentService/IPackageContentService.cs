using DPMGallery.DTO;
using DPMGallery.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Services
{

    public enum DownloadFileType
    {
        dpkg,
        dspec,
        readme,
        icon
    }

    public static class DownloadFileTypeExtensions
    {
        public static string ToContentType(this DownloadFileType fileType)
        {
            switch (fileType)
            {
                case DownloadFileType.dpkg:
                    return "application/octet-stream";
                case DownloadFileType.dspec:
                    return "application/json";
                case DownloadFileType.icon:
                    return "image/xyz";
                case DownloadFileType.readme:
                    return "text/markdown";
                default:
                    return "application/octet-stream";
            }
        }
    }

    public interface IPackageContentService
    {
        Task<PackageVersionsResponseDTO> GetPackageVersionsOrNullAsync(string packageId, CompilerVersion compilerVersion, Platform platform, CancellationToken cancellationToken);

        Task<PackageVersionsWithDependenciesResponseDTO> GetPackageVersionsWithDependenciesOrNullAsync(string packageId, CompilerVersion compilerVersion, Platform platform, CancellationToken cancellationToken, bool listed = true);

        Task<bool> GetPackageVersionExistsAsync(string packageId, string version, CompilerVersion compilerVersion, Platform platform, CancellationToken cancellationToken);

        Task<Stream> GetPackageStreamAsync(DownloadFileType fileType, string id, CompilerVersion compilerVersion, Platform platform, string version, CancellationToken cancellationToken);

    }
}
