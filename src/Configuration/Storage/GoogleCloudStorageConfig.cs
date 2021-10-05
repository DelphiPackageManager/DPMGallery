using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DPMGallery.Configuration
{
    public class GoogleCloudStorageConfig
    {
        [Required]
        public string BucketName { get; set; }
    }
}
