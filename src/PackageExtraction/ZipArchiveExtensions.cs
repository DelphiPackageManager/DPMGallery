using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DPMGallery.PackageExtraction
{
    public static class ZipArchiveExtensions
    {
        public static ZipArchiveEntry LookupEntry(this ZipArchive zipArchive, string path)
        {
            var entry = zipArchive.Entries.FirstOrDefault(zipEntry => UnescapePath(zipEntry.FullName) == path);
            if (entry == null)
            {
                throw new FileNotFoundException(path);
            }

            return entry;
        }

        public static IEnumerable<string> GetFiles(this ZipArchive zipArchive)
        {
            return zipArchive.Entries.Select(e => UnescapePath(e.FullName));
        }

        private static string UnescapePath(string path)
        {
            if (path != null && path.IndexOf('%', StringComparison.Ordinal) > -1)
            {
                return Uri.UnescapeDataString(path);
            }

            return path;
        }

        public static Stream OpenFile(this ZipArchive zipArchive, string path)
        {
            var entry = LookupEntry(zipArchive, path);
            return entry.Open();
        }

        public static string SaveAsFile(this ZipArchiveEntry entry, string fileFullPath, ILogger logger)
        {
            using (var inputStream = entry.Open())
            {
                inputStream.CopyToFile(fileFullPath);
            }

            entry.UpdateFileTimeFromEntry(fileFullPath, logger);

            return fileFullPath;
        }

        public static void UpdateFileTimeFromEntry(this ZipArchiveEntry entry, string fileFullPath, ILogger logger)
        {
            var attr = File.GetAttributes(fileFullPath);

            if (!attr.HasFlag(FileAttributes.Directory) &&
                entry.LastWriteTime.DateTime != DateTime.MinValue && // Ignore invalid times
                entry.LastWriteTime.UtcDateTime <= DateTime.UtcNow) // Ignore future times
            {
                try
                {
                    File.SetLastWriteTimeUtc(fileFullPath, entry.LastWriteTime.Add(entry.LastWriteTime.Offset).UtcDateTime);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    string message = string.Format(
                        CultureInfo.CurrentCulture,
                        "Failed to update File Time on {0} : {1}",
                        fileFullPath, // {0}
                        ex.Message); // {1}

                    logger.Verbose(message);
                }
            }
        }
    }
}
