using BlushingPenguin.JsonPath;
using DPMGallery.Entities;
using DPMGallery.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DPMGallery.PackageExtraction
{
    public class DSpecReader : IDisposable
    {

        private readonly JsonDocument _jsonDocument;
        private bool disposedValue;

        public DSpecReader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            _jsonDocument = JsonDocument.Parse(stream);
            stream.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _jsonDocument.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DSpecReader()
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

        public string GetId()
        {
            var element = _jsonDocument.SelectToken("metadata.id");
            if (element == null)
                throw new Exception("id field in package metadata is empty");
            return element?.GetString();
        }
        public string GetVersion()
        {
            var element = _jsonDocument.SelectToken("metadata.version");
            if (element == null)
                throw new Exception("version field in package metadata is empty");
            return element?.GetString();
        }

        public string GetDescription()
        {
            var element = _jsonDocument.SelectToken("metadata.description");
            if (element == null)
                throw new Exception("description field in package metadata is empty");
            return element?.GetString();
        }

        public string GetAuthors()
        {
            var element = _jsonDocument.SelectToken("metadata.authors");
            if (element == null)
                throw new Exception("authors field in package metadata is empty");
            return element?.GetString();
        }

        public CompilerVersion GetCompilerVersion()
        {
            var element = _jsonDocument.SelectToken("targetPlatforms[0].compiler");
            if (element == null)
                throw new Exception("compiler field in package metadata is empty");
            string compiler = element?.GetString();

            return compiler.ToCompilerVersion();
        }

        public Platform GetPlatform()
        {
            var element = _jsonDocument.SelectToken("targetPlatforms[0].platforms");
            if (element == null)
                throw new Exception("platforms field in package metadata is empty");
            string platform = element?.GetString();

            return platform.ToPlatform();
        }

        public string GetProjectUrl()
        {
            var element = _jsonDocument.SelectToken("metadata.projectUrl");
            return element?.GetString();
        }
        public string GetRepositoryUrl()
        {
            var element = _jsonDocument.SelectToken("metadata.repositoryUrl");
            return element?.GetString();
        }
        public string GetRepositoryType()
        {
            var element = _jsonDocument.SelectToken("metadata.repositoryType");
            return element?.GetString();
        }

        public string GetRepositoryBranch()
        {
            var element = _jsonDocument.SelectToken("metadata.repositoryBranch");
            return element?.GetString();
        }

        public string GetRepositoryCommit()
        {
            var element = _jsonDocument.SelectToken("metadata.repositoryCommit");
            return element?.GetString();
        }

        public string GetLicense()
        {
            var element = _jsonDocument.SelectToken("metadata.license");
            return element?.GetString();
        }

        public string GetIcon()
        {
            var element = _jsonDocument.SelectToken("metadata.icon");
            return element?.GetString();
        }
        public string GetCopyright()
        {
            var element = _jsonDocument.SelectToken("metadata.copyright");
            return element?.GetString();
        }

        public string GetTags()
        {
            var element = _jsonDocument.SelectToken("metadata.tags");
            return element?.GetString();
        }

        public string GetReadMe()
        {
            var element = _jsonDocument.SelectToken("metadata.readme");
            return element?.GetString();
        }
        public string GetReleaseNotes()
        {
            var element = _jsonDocument.SelectToken("metadata.releaseNotes");
            return element?.GetString();
        }

        public bool GetIsTrial()
        {
            var element = _jsonDocument.SelectToken("metadata.isTrial");
            if (element == null)
                return false;
            return Boolean.Parse(element?.GetString());
        }

        public bool GetIsCommercial()
        {
            var element = _jsonDocument.SelectToken("metadata.isCommercial");
            if (element == null)
                return false;
            return Boolean.Parse(element?.GetString());
        }

        public IList<PackageDependency> GetDependencies()
        {
            var result = new List<PackageDependency>();
            var dependenciesElement = _jsonDocument.SelectToken("targetPlatforms[0].dependencies");
            if (!dependenciesElement.HasValue)
                return result;
            var enumerator = dependenciesElement.Value.EnumerateArray();
            foreach (var item in enumerator)
            {

                var idElement = item.GetProperty("id");
                var versionElement = item.GetProperty("version");
                var dependency = new PackageDependency()
                {
                    PackageId = idElement.GetString(),
                    VersionRange = versionElement.GetString()
                };
                result.Add(dependency);
            }
            return result;
        }

    }
}
