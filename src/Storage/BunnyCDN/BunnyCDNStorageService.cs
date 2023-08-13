using DPMGallery.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Net.Http.Headers;

namespace DPMGallery.Storage.BunnyCDN
{
    public class BunnyCDNStorageService : IStorageService, IDisposable
    {
        private readonly ServerConfig _serverConfig;
        private readonly HttpClient _httpClient;
        private bool disposedValue;

        public BunnyCDNStorageService(ServerConfig serverConfig)
        {
            _serverConfig = serverConfig;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("https://storage.bunnycdn.com/");
            _httpClient.Timeout = new TimeSpan(0, 0, 120);
            _httpClient.DefaultRequestHeaders.Add("AccessKey", _serverConfig.Storage.BunnyCDNStorage.ApiAccessKey);


        }

        // GetAsync is only used by the filesystem storage
        public Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public bool IsCDN()
        {
            return true;
        }

        public string NormalizePath(string path, bool? isDirectory = null)
        {
            // Trim all prepending & tailing whitespace, fix windows-like paths then remove prepending slashes
            path = path.Trim()
                .Replace("\\", "/")
                .TrimStart('/');


            if (isDirectory.HasValue)
            {
                if (isDirectory.Value)
                    path = path.TrimEnd('/') + "/";
                else if (path.EndsWith("/"))
                    throw new Exception("The requested path is invalid, cannot be directory.");
            }

            while (path.Contains("//"))
                path = path.Replace("//", "/");

            return path;
        }

        public async Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
        {

            var uploadPath = Path.Combine(_serverConfig.Storage.BunnyCDNStorage.StorageZoneName, path);
            uploadPath = NormalizePath(uploadPath, isDirectory: false);


            //this will cause the stream to be disposed

            try
            {
                var streamCopy = new MemoryStream();
                await content.CopyToAsync(streamCopy, cancellationToken);
                streamCopy.Seek(0, SeekOrigin.Begin);

                using (var contentStream = new StreamContent(streamCopy))
                {
                    //contentStream.Headers.Add("Content-Type", contentType);
                    var message = new HttpRequestMessage(HttpMethod.Put, uploadPath)
                    {
                        Content = contentStream,
                        
                    };

                    message.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    var response = await _httpClient.SendAsync(message);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
                return StoragePutResult.Success;
            }
            catch
            {
                return StoragePutResult.Error;

            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
