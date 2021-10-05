using FluentMigrator.Runner.VersionTableInfo;

namespace DPMGallery.DBMigration
{
    public class VersionTable : IVersionTableMetaData
    {
        public string ColumnName
        {
            get { return "version"; }
        }

        public string SchemaName
        {
            get { return string.Empty; }
        }

        public string TableName
        {
            get { return "versioninfo"; }
        }

        public string UniqueIndexName
        {
            get { return "uc_version"; }
        }

        public string AppliedOnColumnName
        {
            get { return "appliedon"; }
        }

        public string DescriptionColumnName
        {
            get { return "description"; }
        }

        private object _applicationContext;
        public object ApplicationContext
        {
            get => ApplicationContext1;
            set => ApplicationContext1 = value;
        }

        public bool OwnsSchema => true;

        public object ApplicationContext1 { get => _applicationContext; set => _applicationContext = value; }
    }
}
