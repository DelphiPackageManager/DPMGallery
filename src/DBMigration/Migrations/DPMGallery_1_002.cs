using FluentMigrator;

using FL = DPMGallery.Constants.Database.FieldLength;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;

using DB = DPMGallery.Constants.Database;

namespace DPMGallery.DBMigration.Migrations
{

    [Migration(2, "Added active to packages table")]
    public class DPMGallery_1_002 : Migration
    {
         public override void Down()
        {
            
        }

        public override void Up()
        {
			Alter.Table(T.Package).AddColumn("active").AsBoolean().WithDefaultValue(false).SetExistingRowsTo(true);
        }
    }
}
