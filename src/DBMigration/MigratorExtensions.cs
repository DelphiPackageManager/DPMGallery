using FluentMigrator.Builders.Create.Table;

namespace DPMGallery.DBMigration
{
    public static class MigratorExtensions
    {
        public static ICreateTableColumnOptionOrWithColumnSyntax WithGuidPrimaryKeyColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax)
        {
            return tableWithColumnSyntax
                .WithColumn("id")
                .AsGuid()
                .NotNullable()
                .PrimaryKey();
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithIntPrimaryKeyColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax)
        {
            return tableWithColumnSyntax
                .WithColumn("id")
                .AsInt32()
                .NotNullable()
                .PrimaryKey();
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithNameColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax)
        {
            return tableWithColumnSyntax
                .WithColumn("name")
                .AsString(FieldLengths.FieldLengthShort)
                .NotNullable();
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithDescriptionColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax)
        {
            return tableWithColumnSyntax
                .WithColumn("description")
                .AsString(FieldLengths.FieldLengthLong)
                .Nullable();
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithArchivedColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax)
        {
            return tableWithColumnSyntax
                .WithColumn("archived")
                .AsBoolean()
                .NotNullable();
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithVersionColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax)
        {
            return tableWithColumnSyntax
                .WithColumn("version")
                .AsInt32()
                .NotNullable();
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithEnumColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax, string name)
        {
            return tableWithColumnSyntax
                .WithColumn(name)
                .AsInt32()
                .NotNullable();
        }
    }
}
