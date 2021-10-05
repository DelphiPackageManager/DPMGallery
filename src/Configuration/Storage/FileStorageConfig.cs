using System;
using System.Linq;

namespace DPMGallery.Configuration
{
    public class FileStorageConfig
    {

        private static readonly string _defaultFilePath;

        public string Path { get; set; }

        static FileStorageConfig()
        {
            string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            _defaultFilePath = System.IO.Path.Combine(commonAppDataPath, "dpm","dpmserver","packages");

        }

        public FileStorageConfig()
        {
            Path = _defaultFilePath;
        }
    }
}
