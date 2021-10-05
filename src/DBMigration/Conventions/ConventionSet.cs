using DPMGallery.DBMigration.Conventions;
using FluentMigrator.Runner.Conventions;
using System.Collections.Generic;


namespace DPMGallery.DBMigration
{
    public class DPMGalleryConventionSet : IConventionSet
    {
        public DPMGalleryConventionSet()
        {
            var schemaConvention = new DefaultSchemaConvention(new DefaultSchemaNameConvention(null));

            ColumnsConventions = new List<IColumnsConvention>()
            {
                new DPMGalleryPrimaryKeyNameConvention(),
            };

            ConstraintConventions = new List<IConstraintConvention>()
            {
                new DPMGalleryConstraintNameConvention(),
                schemaConvention,
            };

            ForeignKeyConventions = new List<IForeignKeyConvention>()
            {
                new DPMGalleryForeignKeyNameConvention(),
                schemaConvention,
            };

            IndexConventions = new List<IIndexConvention>()
            {
                new DPMGalleryIndexNameConvention(),
                schemaConvention,
            };

            SequenceConventions = new List<ISequenceConvention>()
            {
                schemaConvention,
            };

            AutoNameConventions = new List<IAutoNameConvention>()
            {
                new DefaultAutoNameConvention(),
            };

            SchemaConvention = schemaConvention;
            RootPathConvention = new DefaultRootPathConvention(null);
        }

        public IRootPathConvention RootPathConvention { get; }

        public DefaultSchemaConvention SchemaConvention { get; }

        public IList<IColumnsConvention> ColumnsConventions { get; }

        public IList<IConstraintConvention> ConstraintConventions { get; }

        public IList<IForeignKeyConvention> ForeignKeyConventions { get; }

        public IList<IIndexConvention> IndexConventions { get; }

        public IList<ISequenceConvention> SequenceConventions { get; }

        public IList<IAutoNameConvention> AutoNameConventions { get; }
    }

}
