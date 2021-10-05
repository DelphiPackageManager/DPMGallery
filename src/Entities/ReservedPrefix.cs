using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.ReservedPrefix)]
    public class ReservedPrefix
    {
        [Column("prefix")]
        public string Prefix { get; set; }

        [Column("owner_id")]
        public int OwnerId { get; set; }
    }
}
