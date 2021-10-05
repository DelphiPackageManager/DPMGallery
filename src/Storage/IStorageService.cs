using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Services
{
    //abstracting the Offsite storage provider - s3, azure etc.
    //idea borrowed from BaGet

    public enum StoragePutResult
    {
        /// <summary>
        /// The given path is already used to store different content.
        /// </summary>
        Conflict,

        /// <summary>
        /// This content is already stored at the given path.
        /// </summary>
        AlreadyExists,

        /// <summary>
        /// The content was sucessfully stored.
        /// </summary>
        Success,

        Error
    }


    public interface IStorageService
    {
        bool IsCDN(); 
        Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default);

        Task<Uri> GetDownloadUriAsync(string path, CancellationToken cancellationToken = default);

        Task<StoragePutResult> PutAsync(
            string path,
            Stream content,
            string contentType,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(string path, CancellationToken cancellationToken = default);

        Task CleanFiles(string path, string filter, CancellationToken cancellationToken = default);

        Task DeleteFolderAsync(string folder, CancellationToken cancellationToken = default);
    }
}
