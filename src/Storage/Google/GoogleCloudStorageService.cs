using DPMGallery.Services;
using Google;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Storage
{
    public class GoogleCloudStorageService : IStorageService
    {
        private readonly string _bucketName;

        public GoogleCloudStorageService(ServerConfig serverConfig)
        {
            if (serverConfig == null)
                throw new ArgumentNullException(nameof(serverConfig));

            _bucketName = serverConfig.Storage.GoogleCloudStorage.BucketName;
        }

        public bool IsCDN()
        {
            return true;
        }


        public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            using (var storage = await StorageClient.CreateAsync())
            {
                var stream = new MemoryStream();
                await storage.DownloadObjectAsync(_bucketName, CoercePath(path), stream, cancellationToken: cancellationToken);
                stream.Position = 0;
                return stream;
            }
        }

        public Task<Uri> GetDownloadUriAsync(string path, CancellationToken cancellationToken = default)
        {
            // returns an Authenticated Browser Download URL: https://cloud.google.com/storage/docs/request-endpoints#cookieauth
            return Task.FromResult(new Uri($"https://storage.googleapis.com/{_bucketName}/{CoercePath(path).TrimStart('/')}"));
        }

        public async Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            using (var storage = await StorageClient.CreateAsync())
            using (var seekableContent = new MemoryStream())
            {
                await content.CopyToAsync(seekableContent, 65536, cancellationToken);
                seekableContent.Position = 0;

                var objectName = CoercePath(path);

                try
                {
                    // attempt to upload, succeeding only if the object doesn't exist
                    await storage.UploadObjectAsync(_bucketName, objectName, contentType, seekableContent, new UploadObjectOptions { IfGenerationMatch = 0 }, cancellationToken);
                    return StoragePutResult.Success;
                }
                catch (GoogleApiException e) when (e.HttpStatusCode == HttpStatusCode.PreconditionFailed)
                {
                    // the object already exists; get the hash of its content from its metadata
                    var existingObject = await storage.GetObjectAsync(_bucketName, objectName, cancellationToken: cancellationToken);
                    var existingHash = Convert.FromBase64String(existingObject.Md5Hash);

                    // hash the content that was uploaded
                    seekableContent.Position = 0;
                    byte[] contentHash;
                    using (var md5 = MD5.Create())
                        contentHash = md5.ComputeHash(seekableContent);

                    // conflict if the two hashes are different
                    return existingHash.SequenceEqual(contentHash) ? StoragePutResult.AlreadyExists : StoragePutResult.Conflict;
                }
            }
        }

        public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            using (var storage = await StorageClient.CreateAsync())
            {
                try
                {
                    var obj = await storage.GetObjectAsync(_bucketName, CoercePath(path), cancellationToken: cancellationToken);
                    await storage.DeleteObjectAsync(obj, cancellationToken: cancellationToken);
                }
                catch (GoogleApiException e) when (e.HttpStatusCode == HttpStatusCode.NotFound)
                {
                }
            }
        }

        private static string CoercePath(string path)
        {
            // although Google Cloud Storage objects exist in a flat namespace, using forward slashes allows the objects to
            // be exposed as nested subdirectories, e.g., when browsing via Google Cloud Console
            return path.Replace('\\', '/');
        }

        public Task CleanFiles(string path, string filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFolderAsync(string folder, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
