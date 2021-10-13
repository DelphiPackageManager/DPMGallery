using DPMGallery.DTO;
using DPMGallery.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Versioning;

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
        public static string ToContentType(this DownloadFileType fileType, string ext = "")
        {
            switch (fileType)
            {
                case DownloadFileType.dpkg:
                    return "application/octet-stream";
                case DownloadFileType.dspec:
                    return "application/json";
                case DownloadFileType.icon:
                    if (ext == ".png")
                        return "image/png";
                    if (ext == ".svg")
                        return "image/svg+xml";
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
        //TODO: move to searchservice 
        Task<PackageVersionsResponseDTO> GetPackageVersionsOrNullAsync(string packageId, CompilerVersion compilerVersion, Platform platform, CancellationToken cancellationToken);

        Task<bool> GetPackageVersionExistsAsync(string packageId, string version, CompilerVersion compilerVersion, Platform platform, CancellationToken cancellationToken);

        Task<Stream> GetPackageStreamAsync(DownloadFileType fileType, string id, CompilerVersion compilerVersion, Platform platform, string version, CancellationToken cancellationToken);

        Task<string> GetPackageIconFileExtAsync(string id, CompilerVersion compilerVersion, Platform platform, string version, CancellationToken cancellationToken);
    }
}
