using DPMGallery.Entities;
using NuGet.Versioning;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DPMGallery.PackageExtraction
{
    public class PackageArchiveReader : IDisposable
    {
        private readonly ZipArchive _zipArchive;
        private bool disposedValue;

        private DSpecReader _dSpecReader;

        private const string _dspecFileName = "package.dspec";

        protected Stream ZipReadStream { get; set; }

        public virtual DSpecReader DSpecReader
        {
            get
            {
                if (_dSpecReader == null)
                {
                    _dSpecReader = new DSpecReader(GetDspec());
                }

                return _dSpecReader;
            }
        }

        public PackageArchiveReader(Stream stream)
        {
            _zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, true);
        }


        //takes a file as we converted the upload stream to a file earlier!
        public PackageArchiveReader(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            // Since this constructor owns the stream, the responsibility falls here to dispose the stream of an
            // invalid .zip archive. If this constructor succeeds, the disposal of the stream is handled by the
            // disposal of this instance.
            Stream stream = null;
            try
            {
                stream = File.OpenRead(filePath);
                ZipReadStream = stream;
                _zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
            }
            catch (Exception ex)
            {
                stream?.Dispose();
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Invalid dpkg {0}", filePath), ex);
            }

        }

        public (Package, PackageTargetPlatform, PackageVersion) GetPackageMetaData()
        {
            var package = new Package()
            {
                PackageId = DSpecReader.GetId()
            };

            var targetPlatform = new PackageTargetPlatform()
            {
                CompilerVersion = DSpecReader.GetCompilerVersion(),
                Platform = DSpecReader.GetPlatform(),
            };


            if (!NuGetVersion.TryParseStrict(DSpecReader.GetVersion(), out NuGetVersion version))
                throw new ArgumentNullException("version field in package metadata is not valid");

            var packageVersion = new PackageVersion()
            {
                Version = DSpecReader.GetVersion(),
                Authors = DSpecReader.GetAuthors(),
                Description = DSpecReader.GetDescription(),
                Copyright = DSpecReader.GetCopyright(),
                ProjectUrl = DSpecReader.GetProjectUrl(),
                RepositoryUrl = DSpecReader.GetRepositoryUrl(),
                RepositoryType = DSpecReader.GetRepositoryType(),
                RepositoryBranch = DSpecReader.GetRepositoryBranch(),
                RepositoryCommit = DSpecReader.GetRepositoryCommit(),
                License = DSpecReader.GetLicense(),
                PublishedUtc = DateTime.UtcNow,
                ReadMe = DSpecReader.GetReadMe(),
                ReleaseNotes = DSpecReader.GetReleaseNotes(),
                IsCommercial = DSpecReader.GetIsCommercial(),
                IsTrial = DSpecReader.GetIsTrial(),
                IsPrerelease = version.IsPrerelease,
                Icon = DSpecReader.GetIcon(),
                Tags = DSpecReader.GetTags(),
                Listed = false,
                Dependencies = DSpecReader.GetDependencies(),
                SearchPaths = string.Join(';',DSpecReader.GetSearchPaths())
            };

            //mistakes on old packages, client needs to fix this.
            packageVersion.Tags = packageVersion.Tags.Replace(',', ' ');

            //if we got here, then the package metadata is ok.

            return (package, targetPlatform, packageVersion);
        }


        public Stream GetDspec()
        {
            return GetStream(_dspecFileName);
        }

        public IEnumerable<string> GetFiles()
        {
            return _zipArchive.GetFiles();
        }

        public IEnumerable<string> GetFiles(string folder)
        {
            return GetFiles().Where(f => f.StartsWith(folder + "/", StringComparison.OrdinalIgnoreCase));
        }

        public ZipArchiveEntry GetEntry(string packageFile)
        {
            return _zipArchive.LookupEntry(packageFile);
        }

        public string ExtractFile(string packageFile, string targetFilePath, ILogger logger)
        {
            var entry = GetEntry(packageFile);
            var copiedFile = entry.SaveAsFile(targetFilePath, logger);
            return copiedFile;
        }

        public Stream GetStream(string path)
        {
            Stream stream = null;
            path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (!string.IsNullOrEmpty(path))
            {
                stream = _zipArchive.OpenFile(path);
            }

            return stream;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _zipArchive.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PackageArchiveReader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
