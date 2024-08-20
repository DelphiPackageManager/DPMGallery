using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.ReservedPrefix)]
    public class ReservedPrefix
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("prefix")]
        public string Prefix { get; set; }

        [Column("owner_id")]
        public int OwnerId { get; set; }
    }
}
