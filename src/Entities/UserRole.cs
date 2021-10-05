using Dapper.Contrib.Extensions;
using DPMGallery.Data;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.UserRoles)]
    public class UserRole
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

    }
}
