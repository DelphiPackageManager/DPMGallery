using FluentMigrator;
using FL = DPMGallery.Constants.Database.FieldLength;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;

using DB = DPMGallery.Constants.Database;


namespace DPMGallery.DBMigration.Migrations
{
    [Migration(6, "Rework reserved prefix")]
    public class DPMGallery_1_006 : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Delete.Table(T.ReservedPrefixOwner);
            Delete.Table(T.ReservedPrefix);

            Create.Table(T.ReservedPrefix)
                .WithIntPrimaryKeyColumn().Identity()
                .WithColumn("prefix").AsString(FL.Short, DB.Collation).NotNullable().Unique()
                .WithColumn("owner_id").AsInt32().NotNullable();

            Create.Index("ix_reserved_prefix")
                .OnTable(T.ReservedPrefix)
                .OnColumn("prefix").Unique();


            Create.ForeignKey($"fk_{T.ReservedPrefix}_{T.Users}_owner_id")
                .FromTable(T.ReservedPrefix)
                .ForeignColumn("owner_id")
                .ToTable(T.Users)
                .PrimaryColumn("id");

        }

    }
}
