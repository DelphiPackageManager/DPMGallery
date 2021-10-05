using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DPMGallery.Configuration
{
    public class AmazonS3StorageConfig
    {
        [Required]
        public string AccessKey { get; set; }

        [Required]
        public string SecretKey { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string Bucket { get; set; }

        public string Prefix { get; set; }

        public bool UseInstanceProfile { get; set; }

        public string AssumeRoleArn { get; set; }
    }
}
