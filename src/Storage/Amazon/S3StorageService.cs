using Amazon.S3;
using Amazon.S3.Model;
using DPMGallery.Services;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Storage
{
    public class S3StorageService : IStorageService
    {
        private const string Separator = "/";
        private readonly string _bucket;
        private readonly string _prefix;
        private readonly AmazonS3Client _client;
        private readonly ILogger _logger;
        private readonly bool _disablePayloadSigning;

        public S3StorageService(ILogger logger, ServerConfig serverConfig, AmazonS3Client client)
        {
            _logger = logger;
            if (serverConfig == null)
                throw new ArgumentNullException(nameof(serverConfig));

            _bucket = serverConfig.Storage.S3Storage.Bucket;
            _prefix = serverConfig.Storage.S3Storage.Prefix;
            _disablePayloadSigning = serverConfig.Storage.S3Storage.DisablePayloadSigning;

            _client = client ?? throw new ArgumentNullException(nameof(client));

            if (!string.IsNullOrEmpty(_prefix) && !_prefix.EndsWith(Separator))
                _prefix += Separator;
        }

        public bool IsCDN()
        {
            return true;
        }

        private string PrepareKey(string path)
        {
            return _prefix + path.Replace("\\", Separator);
        }

        public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            var stream = new MemoryStream();

            try
            {
                using (var request = await _client.GetObjectAsync(_bucket, PrepareKey(path), cancellationToken))
                {
                    await request.ResponseStream.CopyToAsync(stream, cancellationToken: cancellationToken);
                }

                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception)
            {
                stream.Dispose();

                // TODO
                throw;
            }

            return stream;
        }

        public Task<Uri> GetDownloadUriAsync(string path, CancellationToken cancellationToken = default)
        {
            var url = _client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucket,
                Key = PrepareKey(path)
            });

            return Task.FromResult(new Uri(url));
        }

        public async Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                //TODO : if it exists already then ovewrite it.
                using (var seekableContent = new MemoryStream())
                {
                    await content.CopyToAsync(seekableContent, 4096, cancellationToken);

                    seekableContent.Seek(0, SeekOrigin.Begin);

                    PutObjectResponse putResponse = await _client.PutObjectAsync(new PutObjectRequest
                    {
                        BucketName = _bucket,
                        Key = PrepareKey(path),
                        InputStream = seekableContent,
                        ContentType = contentType,
                        AutoResetStreamPosition = false,
                        AutoCloseStream = false,
                        DisablePayloadSigning = _disablePayloadSigning,
                    }, cancellationToken);

                    _logger.Information($"S3 Put response : {putResponse.HttpStatusCode}");

                }
                return StoragePutResult.Success;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "[{category}] Error putting file in S3 bucket", "S3StorageService");
                throw;
            }
            
        }

        public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            await _client.DeleteObjectAsync(_bucket, PrepareKey(path), cancellationToken);
        }

        //not sure this is correct!
        public async Task DeleteFolderAsync(string folder, CancellationToken cancellationToken = default)
        {
            await _client.DeleteObjectAsync(_bucket, PrepareKey(folder), cancellationToken);
        }

        public Task CleanFiles(string path, string filter, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

    }
}
