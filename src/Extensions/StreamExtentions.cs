using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Extensions
{
    public static class StreamExtentions
    {

        private const int DefaultCopyBufferSize = 81920;

        /// Copies a stream to a temporary filestream, and returns that file as a stream. 
        /// The file is deleted when closed.
        public static async Task<FileStream> CopyToTemporaryFileStreamAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            var result = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, DefaultCopyBufferSize, FileOptions.DeleteOnClose);
            try
            {
                await stream.CopyToAsync(result, DefaultCopyBufferSize, cancellationToken);
                result.Position = 0;
            }
            catch (Exception)
            {
                result.Dispose();
                throw;
            }

            return result;
        }

        public static bool Matches(this Stream content, Stream target)
        {
            using (var sha256 = SHA256.Create())
            {
                var contentHash = sha256.ComputeHash(content);
                var targetHash = sha256.ComputeHash(target);

                return contentHash.SequenceEqual(targetHash);
            }
        }
    }
}
