using FluentMigrator;

using FL = DPMGallery.Constants.Database.FieldLength;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;

using DB = DPMGallery.Constants.Database;

namespace DPMGallery.DBMigration.Migrations
{

    [Migration(3, "Added revoked to apikey table")]
    public class DPMGallery_1_003 : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Alter.Table(T.ApiKey).AddColumn("revoked").AsBoolean().WithDefaultValue(false).SetExistingRowsTo(false);
        }
    }
}
