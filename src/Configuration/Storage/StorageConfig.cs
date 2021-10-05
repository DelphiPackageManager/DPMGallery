using System;

namespace DPMGallery.Configuration
{
    public enum StorageType
    {
        FileSystem = 0,
        AwsS3 = 1,
        GoogleCloudStorage = 2
    }

    public class StorageConfig
    {
        public StorageType StorageType { get; set; } = StorageType.FileSystem;

        public FileStorageConfig FileStorage { get; set; } = new FileStorageConfig();

        public AmazonS3StorageConfig S3Storage { get; set; } = new AmazonS3StorageConfig();

        public GoogleCloudStorageConfig GoogleCloudStorage { get; set; } = new GoogleCloudStorageConfig();

        public string CDNBaseUri { get; set; } = "https://packages.delphipm.org";

        public int MaxRetries { get; set; } = 3;
    }
}
