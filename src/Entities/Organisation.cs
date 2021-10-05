using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.Users)] //orgs are a special kind of user.
    public class Organisation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_name")]
        public string Name { get; set; }
    }
}
