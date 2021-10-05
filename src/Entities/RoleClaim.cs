using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.RoleClaims)]
    public class RoleClaim
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("role_id")]
        public string RoleId { get; set; }

        [Column("claim_type")]
        public string ClaimType { get; set; }

        [Column("claim_value")]
        public string ClaimValue { get; set; }
    }
}
