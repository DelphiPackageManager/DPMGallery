using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
    public class PackageVersionProcess
    {
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// The packageversion we are operating on
        /// </summary>
        [Column("packageversion_id")]
        public int PackageVersionId { get; set; }
        
        /// <summary>
        /// The filename without the processing folder path
        /// </summary>
        [Column("package_filename")]
        public string PackageFileName { get; set; }

        /// <summary>
        /// set to true when we are done, we can use another background task to clean them 
        /// up periodically.. 
        /// </summary>
        [Column("completed")]
        public bool Completed { get; set; }

        [Column("last_updated_utc")]
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;

        [Column("retry_count")]
        public int RetryCount { get; set; } = 0;
    }
}

