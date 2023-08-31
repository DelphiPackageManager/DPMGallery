using FluentMigrator;

using FL = DPMGallery.Constants.Database.FieldLength;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;

using DB = DPMGallery.Constants.Database;

namespace DPMGallery.DBMigration.Migrations
{

    [Migration(4, "Renamed role column to member_role")]
    public class DPMGallery_1_004 : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Rename.Column("role").OnTable(T.OrganisationMembers).To("member_role");
        }
    }
}
