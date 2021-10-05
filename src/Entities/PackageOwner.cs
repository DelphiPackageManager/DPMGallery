using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.PackageOwner)]
    public class PackageOwner
    {
        [Column("owner_id")]
        public int OwnerId { get; set; }

        [Column("package_id")]
        public int PackageId { get; set; }
    }
}
