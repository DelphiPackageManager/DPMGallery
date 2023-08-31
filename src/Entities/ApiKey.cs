using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.ApiKey)]
    public class ApiKey
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("key_hashed")]
        public string KeyHashed { get; set; } //only used for inserts!

        //do not persist!
        public string Key { get; set; }


        [Column("user_id")]
        public int UserId { get; set; }

        [Column("expires_utc")]
        public DateTime ExpiresUTC { get; set; }

        //either a glob pattern .
        [Column("glob_pattern")]
        public string GlobPattern { get; set; }

        //or a list of packages
        [Column("package_list")]
        public string Packages { get; set; }

        [Column("scopes")]
        public ApiKeyScope Scopes { get; set; }

        [Column("revoked")]
        public bool Revoked { get; set; }
    }
}
