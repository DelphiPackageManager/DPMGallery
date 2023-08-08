using BunnyCDN.Net.Storage;
using DPMGallery.Services;
using Microsoft.VisualBasic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Storage.BunnyCDN
{
    public class BunnyCDNStorageService : IStorageService
    {
        private readonly ServerConfig _serverConfig;
        public BunnyCDNStorageService(ServerConfig serverConfig)
        {
            _serverConfig = serverConfig;
            
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

        public async Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            
            var streamCopy = new MemoryStream();
            await content.CopyToAsync(streamCopy, cancellationToken);

            var bunnyCDNStorage = new BunnyCDNStorage(_serverConfig.Storage.BunnyCDNStorage.StorageZoneName, _serverConfig.Storage.BunnyCDNStorage.ApiAccessKey);
            
            var uploadPath = Path.Combine("//", _serverConfig.Storage.BunnyCDNStorage.StorageZoneName,  path);
            uploadPath = uploadPath.Replace("\\", "/");
            
            try
            {
                //wtf this closes the stream!
                await bunnyCDNStorage.UploadAsync(streamCopy, uploadPath,true); //upload with checksum validation
                return StoragePutResult.Success;
            }
            catch
            {
                return StoragePutResult.Error;

            }
            
        }
    }
}
