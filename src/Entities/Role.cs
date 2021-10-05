using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Claims;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.Roles)]
    public class Role
    {
        //
        // Summary:
        //     Gets or sets the primary key for this role.
        [Column("id")]
        public int Id { get; set; }
        //
        // Summary:
        //     Gets or sets the name for this role.
        [Column("name")]
        public string Name { get; set; }
        //
        // Summary:
        //     Gets or sets the normalized name for this role.
        [Column("normalized_name")]
        public string NormalizedName { get; set; }
        //
        // Summary:
        //     A random value that should change whenever a role is persisted to the store
        [Column("concurrency_stamp")]
        public string ConcurrencyStamp { get; set; }

        //used for identity
        //
        // Summary:
        //     Gets or sets the primary key for this role.
        internal List<Claim> Claims { get; set; }
    }
}
