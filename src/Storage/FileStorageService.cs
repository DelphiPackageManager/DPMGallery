using DPMGallery.Extensions;
using DPMGallery.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace DPMGallery.Storage
{
    //borrowed 
    public class FileStorageService : IStorageService
    {
        private readonly ServerConfig _serverConfig;

        // See: https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/Stream.cs#L35
        private const int DefaultCopyBufferSize = 81920;

        private readonly string _storePath;


        public FileStorageService(ServerConfig serverConfig)
        {
            _serverConfig = serverConfig;
            // Resolve relative path components ('.'/'..') and ensure there is a trailing slash.
            _storePath = Path.GetFullPath(_serverConfig.Storage.FileStorage.Path);
            if (!_storePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                _storePath += Path.DirectorySeparatorChar;
        }

        public bool IsCDN()
        {
            return false;
        }


        private string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path is required", nameof(path));
            }

            var fullPath = Path.GetFullPath(Path.Combine(_storePath, path));

            // Verify path is under the _storePath.
            if (!fullPath.StartsWith(_storePath, StringComparison.Ordinal) ||
                fullPath.Length == _storePath.Length)
            {
                throw new ArgumentException("Path resolves outside store path", nameof(path));
            }

            return fullPath;
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                File.Delete(GetFullPath(path));
            }
            catch (DirectoryNotFoundException)
            {
            }

            return Task.CompletedTask;
        }

        public Task DeleteFolderAsync(string folder, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                Directory.Delete(GetFullPath(folder));
            }
            catch (DirectoryNotFoundException)
            {
            }

            return Task.CompletedTask;
        }

        public Task CleanFiles(string path, string filter, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (string f in Directory.EnumerateFiles(path, filter))
            {
                try
                {
                    File.Delete(f);
                } catch
                {
                    //ignore for now
                }
                
            }

            return  Task.CompletedTask;
        }



        public Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            path = GetFullPath(path);
            try
            {
                var content = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return Task.FromResult<Stream>(content);
            }
            catch
            {
                return Task.FromResult<Stream>(null);
            }
        }

        public Task<Uri> GetDownloadUriAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new Uri(GetFullPath(path));

            return Task.FromResult(result);
        }

        public async Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrEmpty(contentType)) throw new ArgumentException("Content type is required", nameof(contentType));

            cancellationToken.ThrowIfCancellationRequested();

            path = GetFullPath(path);

            // Ensure that the path exists.
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            //try
            //{
                //overrwrite existing file. 
                using (var fileStream = File.Open(path, FileMode.OpenOrCreate))
                {
                    await content.CopyToAsync(fileStream, DefaultCopyBufferSize, cancellationToken);
                    return StoragePutResult.Success;
                }
            //}
            //catch (IOException) when (File.Exists(path))
            //{
            //    using (var targetStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            //    {
            //        content.Position = 0;
            //        return content.Matches(targetStream)
            //            ? StoragePutResult.AlreadyExists
            //            : StoragePutResult.Conflict;
            //    }
            //}
        }
    }
}
