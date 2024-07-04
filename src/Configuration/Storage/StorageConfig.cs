using DPMGallery.Configuration.Storage;
using System;

namespace DPMGallery.Configuration
{
    public enum StorageType
    {
        FileSystem = 0,
        S3Service = 1,
        GoogleCloudStorage = 2,
        BunnyCDN = 3,
    }

    public class StorageConfig
    {
        public StorageType StorageType { get; set; } = StorageType.FileSystem;

        public FileStorageConfig FileStorage { get; set; } = new FileStorageConfig();

        public AmazonS3StorageConfig S3Storage { get; set; } = new AmazonS3StorageConfig();

        public GoogleCloudStorageConfig GoogleCloudStorage { get; set; } = new GoogleCloudStorageConfig();

        public BunnyCDNStorageConfig BunnyCDNStorage { get; set; }  = new BunnyCDNStorageConfig();
        public string CDNBaseUri { get; set; } = "https://packages.delphi.dev";

        public int MaxRetries { get; set; } = 3;
    }
}
